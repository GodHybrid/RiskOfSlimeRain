using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RiskOfSlimeRain.Effects;
using RiskOfSlimeRain.Helpers;
using System.Collections.Generic;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace RiskOfSlimeRain
{
	public static class RORInterfaceLayers
	{
		private const int INVENTORY_SIZE = 47;

		public static string Name => ModContent.GetInstance<RiskOfSlimeRainMod>().Name;

		public static int hoverIndex = -1;

		//Multiplayer syncing thing used when changing stack manually from the UI. Keeps track of any changed stacks and a timer
		//When timer runs out, sync
		/// <summary>
		/// Value is Ref(int) because it's counted down from within the iteration loop
		/// </summary>
		public static Dictionary<int, Ref<int>> TimeByIndex { get; set; }

		private const int syncTimer = 25;

		public static void Load()
		{
			TimeByIndex = new Dictionary<int, Ref<int>>();
		}

		public static void Unload()
		{
			TimeByIndex = null;
		}

		public static void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			if (Main.gameMenu) return;
			int inventoryIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));
			if (inventoryIndex != -1)
			{
				layers.Insert(inventoryIndex, new LegacyGameInterfaceLayer(
					$"{Name}: {nameof(Effects)}",
					Effects,
					InterfaceScaleType.UI
				));
			}
		}

		private static readonly GameInterfaceDrawMethod Effects = delegate
		{
			Player player = Main.LocalPlayer;
			List<ROREffect> effects = player.GetRORPlayer().Effects;
			if (effects.Count == 0) return true;

			int xPosition = 0;
			int yPosition = 0;
			Rectangle sourceRect;
			Rectangle destRect;

			int numHorizontal = (Main.screenWidth - 3 * INVENTORY_SIZE) / 38;
			int lineOffset = 0;
			int numLines = effects.Count / (numHorizontal + 1);
			int yStart = Main.screenHeight - 30 - numLines * 50;
			ROREffect effect;
			Texture2D texture;

			for (int i = 0; i < effects.Count; i++)
			{
				effect = effects[i];
				texture = ModContent.GetTexture(effect.Texture);

				lineOffset = i / numHorizontal;
				//2 * INVENTORY_SIZE is the distance needed to not overlap with recipe UI
				xPosition = 2 * INVENTORY_SIZE + (i - lineOffset * numHorizontal) * 38;

				yPosition = yStart + lineOffset * 50;

				sourceRect = texture.Bounds;
				int width = sourceRect.Width;
				int height = sourceRect.Height;
				destRect = Utils.CenteredRectangle(new Vector2(xPosition, yPosition), new Vector2(width, height));
				Vector2 Bottom = new Vector2(xPosition, yPosition) + Main.screenPosition;
				//destRect.Y = (int)yPosition - height;
				if (i == 0)
				{
					//Utils.DrawLine(Main.spriteBatch, Bottom, new Vector2(Bottom.X + 500, Bottom.Y), Color.White, Color.White, 1);
				}
				//Utils.DrawLine(Main.spriteBatch, new Vector2(Center.X -1, Center.Y - 1), new Vector2(Center.X + 2, Center.Y + 2), Color.Green, Color.White, 4);

				Color color = Color.White * 0.8f;
				if (Config.Instance.CustomStacking && destRect.Contains(new Point(Main.mouseX, Main.mouseY)))
				{
					hoverIndex = i;
					destRect.Inflate(2, 2);
					color = Color.White;
				}

				Main.spriteBatch.Draw(texture, destRect, sourceRect, color);

				//Vector2 bottomCenter = destRect.BottomLeft();
				Vector2 bottomCenter = new Vector2(xPosition - width / 2, yPosition + 14);
				string text = "x" + effect.Stack.ToString();
				if (!effect.FullStack) text += "/" + effect.UnlockedStack;
				Vector2 length = Main.fontItemStack.MeasureString(text);
				bottomCenter.Y -= length.Y / 2;
				color = Color.White;
				if (effect.Capped)
				{
					color = Color.LawnGreen;
				}
				ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontItemStack, text, bottomCenter, color, 0, Vector2.Zero, Vector2.One * 0.78f);
			}

			if (hoverIndex > -1)
			{
				effect = effects[hoverIndex];
				Main.hoverItemName = effect.Description;
				if (effect.UIInfo != string.Empty)
				{
					Main.hoverItemName += "\n" + effect.UIInfo;
				}
				if (effect.Capped)
				{
					Main.hoverItemName += "\n" + effect.CappedMessage;
				}
			}

			return true;
		};

		public static void Update(Player player)
		{
			if (Main.myPlayer == player.whoAmI && hoverIndex != -1 && !player.mouseInterface)
			{
				RORPlayer mPlayer = player.GetRORPlayer();
				List<ROREffect> effects = mPlayer.Effects;
				//this stuff is here cause only here resetting scrollwheel status works properly
				int oldStack = effects[hoverIndex].Stack;
				if (PlayerInput.ScrollWheelDelta > 0)
				{
					effects[hoverIndex].Stack++;
					PlayerInput.ScrollWheelDelta = 0;
					Main.PlaySound(SoundID.MenuTick, volumeScale: 0.8f);
				}
				else if (PlayerInput.ScrollWheelDelta < 0)
				{
					effects[hoverIndex].Stack--;
					PlayerInput.ScrollWheelDelta = 0;
					Main.PlaySound(SoundID.MenuTick, volumeScale: 0.8f);
				}
				if (Main.netMode != NetmodeID.SinglePlayer && oldStack != effects[hoverIndex].Stack)
				{
					SetChangedEffect(hoverIndex);
				}

				hoverIndex = -1;
			}

			if (Main.myPlayer == player.whoAmI && Main.netMode != NetmodeID.SinglePlayer)
			{
				SyncChangedEffects(player);
			}
		}

		private static void SetChangedEffect(int index)
		{
			if (TimeByIndex.ContainsKey(index))
			{
				TimeByIndex[index].Value = syncTimer;
			}
			else
			{
				TimeByIndex.Add(index, new Ref<int>(syncTimer));
			}
		}

		private static void SyncChangedEffects(Player player)
		{
			RORPlayer mPlayer = player.GetRORPlayer();
			List<ROREffect> effects = mPlayer.Effects;
			List<int> toRemove = new List<int>();
			foreach (int index in TimeByIndex.Keys)
			{
				if (TimeByIndex[index].Value <= 0)
				{
					ROREffectManager.SendSingleEffectStack((byte)player.whoAmI, index, effects[index]);
					toRemove.Add(index);
				}
			}
			for (int i = 0; i < toRemove.Count; i++)
			{
				TimeByIndex.Remove(toRemove[i]);
			}
			foreach (int index in TimeByIndex.Keys)
			{
				TimeByIndex[index].Value--;
			}
		}
	}
}
