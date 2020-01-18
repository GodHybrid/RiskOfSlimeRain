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
using Terraria.DataStructures;
using RiskOfSlimeRain.Effects.Common;

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
			On.Terraria.Main.DrawPlayer_DrawAllLayers += Main_DrawPlayer_DrawAllLayers;
		}

		public static void Unload()
		{
			TimeByIndex = null;
		}

		public static void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			if (Main.gameMenu) return;
			int mouseIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
			if (mouseIndex != -1)
			{
				layers.Insert(mouseIndex, new LegacyGameInterfaceLayer(
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
				if (destRect.Contains(new Point(Main.mouseX, Main.mouseY)))
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
				if (Main.hoverItemName != "" || player.mouseInterface || Main.mouseText) return true;
				player.showItemIcon = false;
				effect = effects[hoverIndex];
				string name = effect.Name;
				string text = "\n" + effect.Description;
				if (effect.UIInfo != string.Empty)
				{
					text += "\n" + effect.UIInfo;
				}
				if (effect.Capped)
				{
					text += "\n" + effect.CappedMessage;
				}

				Vector2 mousePos = new Vector2(Main.mouseX, Main.mouseY);
				mousePos += new Vector2(Main.ThickMouse ? 22 : 16);

				Vector2 size = Main.fontMouseText.MeasureString(text);

				if (mousePos.X + size.X + 4f > Main.screenWidth)
				{
					mousePos.X = (int)(Main.screenWidth - size.X - 4f);
				}
				if (mousePos.Y + size.Y + 4f > Main.screenHeight)
				{
					mousePos.Y = (int)(Main.screenHeight - size.Y - 4f);
				}

				ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontMouseText, name, mousePos, effect.RarityColor * (Main.mouseTextColor / 255f), 0, Vector2.Zero, Vector2.One);
				ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontMouseText, text, mousePos, Color.White * (Main.mouseTextColor / 255f), 0, Vector2.Zero, Vector2.One);
			}

			return true;
		};

		private static void Main_DrawPlayer_DrawAllLayers(On.Terraria.Main.orig_DrawPlayer_DrawAllLayers orig, Main self, Player drawPlayer, int projectileDrawPosition, int cHead)
		{
			SoldiersSyringeEffect effect = ROREffectManager.GetEffectOfType<SoldiersSyringeEffect>(drawPlayer.GetRORPlayer());
			if (effect == null || effect?.Active == false)
			{
				orig(self, drawPlayer, projectileDrawPosition, cHead);
				return;
			}

			//Modify Main.playerDrawData first
			List<DrawData> drawDatas = new List<DrawData>(Main.playerDrawData);

			//Get the layers that contain the data about where they are from
			List<PlayerLayer> layers = PlayerHooks.GetDrawLayers(drawPlayer);

			//Exclude those layers that we add (because drawDatas is just raw data, no indication of where it's from)
			List<int> ourModded = new List<int>();
			for (int i = 0; i < layers.Count; i++)
			{
				if (layers[i].mod == RiskOfSlimeRainMod.Instance.Name)
				{
					ourModded.Add(i);
				}
			}

			for (int i = 0; i < drawDatas.Count; i++)
			{
				if (ourModded.Contains(i)) continue;
				DrawData data = drawDatas[i];
				//data.position += shakePosOffset * shakeTimer;
				//data.scale += shakeScaleOffset * shakeTimer;
				data.position += effect.shakePosOffset * effect.shakeTimer;
				data.scale += effect.shakeScaleOffset * effect.shakeTimer;
				data.color = data.color.MultiplyRGBA(Color.Yellow * 0.5f);
				data.color *= 0.05f * effect.shakeTimer;
				drawDatas[i] = data;
				//data.color *= 0.5f;
			}

			Main.playerDrawData.AddRange(drawDatas);

			orig(self, drawPlayer, projectileDrawPosition, cHead);
		}

		public static void Update(Player player)
		{
			if (!Config.Instance.CustomStacking) return;
			if (Main.myPlayer == player.whoAmI && hoverIndex != -1)
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
					ROREffectManager.SendSingleEffectStack(mPlayer, effects[index]);
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
