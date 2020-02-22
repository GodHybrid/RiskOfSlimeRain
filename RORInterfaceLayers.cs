﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RiskOfSlimeRain.Core.ROREffects;
using RiskOfSlimeRain.Core.ROREffects.Common;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Network.Effects;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace RiskOfSlimeRain
{
	public static class RORInterfaceLayers
	{
		private const int inventorySize = 47;

		private const int iconSize = 32;
		private const int iconPadding = iconSize + 6;
		private const int verticalLineHeight = 50;

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
			if (!Main.playerInventory && Config.Instance.OnlyShowWhenOpenInventory)
			{
				return true;
			}
			Player player = Main.LocalPlayer;
			List<ROREffect> effects = player.GetRORPlayer().Effects;
			if (effects.Count == 0) return true;

			int xPosition = 0;
			int yPosition = 0;
			Rectangle sourceRect;
			Rectangle destRect;


			//int initialVerticalOffset = Main.screenHeight - 30;
			int initialVerticalOffset = (int)(Main.screenHeight * (1 - Config.Instance.ItemUIVerticalOffset));

			int numHorizontal = (Main.screenWidth - 3 * inventorySize) / iconPadding;
			if (numHorizontal == 0) numHorizontal = 1;
			int lineOffset = 0;
			int numLines = effects.Count / (numHorizontal + 1);
			int yStart = initialVerticalOffset - numLines * verticalLineHeight;
			ROREffect effect;
			Texture2D texture;

			//Draw the info icon on the very left
			xPosition = 2 * inventorySize + (-1 - 0) * iconPadding;
			yPosition = yStart + 0;
			bool drawingMisc = true;


			for (int i = 0; i < effects.Count; i++)
			{
				effect = effects[i];
				texture = ModContent.GetTexture(effect.Texture);

				lineOffset = i / numHorizontal;
				//2 * INVENTORY_SIZE is the distance needed to not overlap with recipe UI
				xPosition = 2 * inventorySize + (i - lineOffset * numHorizontal) * iconPadding;

				yPosition = yStart + lineOffset * verticalLineHeight;

				if (drawingMisc)
				{
					texture = Main.npcHeadTexture[0];
					xPosition -= iconPadding;
				}

				sourceRect = texture.Bounds;
				int width = sourceRect.Width;
				int height = sourceRect.Height;

				//SCALING
				float scale = 1f;
				if (width > height) scale = (float)iconSize / width;
				else scale = (float)iconSize / height;

				width = (int)(width * scale);
				height = (int)(height * scale);

				destRect = Utils.CenteredRectangle(new Vector2(xPosition, yPosition), new Vector2(width, height));

				Vector2 Bottom = new Vector2(xPosition, yPosition) + Main.screenPosition;

				//destRect.Y = (int)yPosition - height;
				//if (i == 0)
				//{
				//	Utils.DrawLine(Main.spriteBatch, Bottom, new Vector2(Bottom.X + 500, Bottom.Y), Color.White, Color.White, 1);
				//}
				//Vector2 Center = destRect.Center.ToVector2() + Main.screenPosition;
				//Utils.DrawLine(Main.spriteBatch, new Vector2(Center.X - 10, Center.Y - 10), new Vector2(Center.X + 20, Center.Y + 20), Color.Green, Color.White, 4);

				Color color = Color.White * 0.8f;

				if (drawingMisc)
				{
					color = Color.White;
				}

				Rectangle mouseCheck = Utils.CenteredRectangle(new Vector2(xPosition, yPosition), new Vector2(32));
				if (mouseCheck.Contains(new Point(Main.mouseX, Main.mouseY)))
				{
					hoverIndex = i;
					destRect.Inflate(2, 2);
					color = Color.White;
					if (drawingMisc)
					{
						hoverIndex = -2;
					}
				}

				Main.spriteBatch.Draw(texture, destRect, sourceRect, color);

				Vector2 bottomCenter = new Vector2(xPosition - width / 2, yPosition + 14);
				//Vector2 bottomCenter = destRect.BottomLeft();

				if (drawingMisc)
				{
					drawingMisc = false;
					i--;
				}
				else
				{
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

				Vector2 textPos = SetTextPos(text);

				ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontMouseText, name, textPos, effect.RarityColor * (Main.mouseTextColor / 255f), 0, Vector2.Zero, Vector2.One);
				ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontMouseText, text, textPos, Color.White * (Main.mouseTextColor / 255f), 0, Vector2.Zero, Vector2.One);
			}
			else if (hoverIndex == -2) //Misc
			{
				if (Main.hoverItemName != "" || player.mouseInterface || Main.mouseText) return true;
				player.showItemIcon = false;

				string text = $"This UI shows all your currently used items from '{RiskOfSlimeRainMod.Instance.DisplayName}'";
				text += "\nCheck the config of this mod to customize the UI";
				text += "\nMisc Info:";
				text += "\nProc multiplier (based on held weapon): " + ROREffect.GetProcByUseTime(player).ToPercent(2);
				text += "\nItem drop chance from bosses: " + RORWorld.DropChance.ToPercent(2);

				Vector2 textPos = SetTextPos(text);

				ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontMouseText, text, textPos, Color.White * (Main.mouseTextColor / 255f), 0, Vector2.Zero, Vector2.One);
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

			for (int i = 0; i < drawDatas.Count; i++)
			{
				DrawData data = drawDatas[i];
				if (data.texture.Name.StartsWith("RiskOfSlimeRain")) continue;
				data.position += effect.shakePosOffset * effect.shakeTimer;
				data.scale += effect.shakeScaleOffset * effect.shakeTimer;
				data.color = data.color.MultiplyRGBA(Color.Yellow * 0.5f);
				data.color *= 0.05f * effect.shakeTimer;
				drawDatas[i] = data;
			}

			Main.playerDrawData.AddRange(drawDatas);

			orig(self, drawPlayer, projectileDrawPosition, cHead);
		}

		/// <summary>
		/// Returns mouse position based on text and screen edges
		/// </summary>
		private static Vector2 SetTextPos(string text)
		{
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
			return mousePos;
		}

		public static void Update(Player player)
		{
			if (Main.myPlayer == player.whoAmI && hoverIndex != -1)
			{
				if (!Config.Instance.CustomStacking || hoverIndex == -2)
				{
					hoverIndex = -1;
					return;
				}
				RORPlayer mPlayer = player.GetRORPlayer();
				List<ROREffect> effects = mPlayer.Effects;
				//This stuff is here cause only here resetting scrollwheel status works properly
				ROREffect effect = effects[hoverIndex];
				int oldStack = effect.Stack;
				if (PlayerInput.ScrollWheelDelta > 0)
				{
					effect.Stack++;
					PlayerInput.ScrollWheelDelta = 0;
					Main.PlaySound(SoundID.MenuTick, volumeScale: 0.8f);
				}
				else if (PlayerInput.ScrollWheelDelta < 0)
				{
					effect.Stack--;
					PlayerInput.ScrollWheelDelta = 0;
					Main.PlaySound(SoundID.MenuTick, volumeScale: 0.8f);
				}
				if (Main.netMode != NetmodeID.SinglePlayer && oldStack != effect.Stack)
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
					new ROREffectSyncSingleStackPacket(effects[index]).Send();
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
