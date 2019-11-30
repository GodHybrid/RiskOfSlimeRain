using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RiskOfSlimeRain.Items.Consumable;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace RiskOfSlimeRain
{
	public static class RORInterfaceLayers
	{
		internal const int INVENTORY_SIZE = 47;
		internal const int SHOP_SIZE = 40;
		public static string Name => ModContent.GetInstance<RiskOfSlimeRain>().Name;
		internal static void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			if (Main.gameMenu) return;
			int InventoryIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));
			if (InventoryIndex != -1)
			{
				layers.Insert(InventoryIndex + 2, new LegacyGameInterfaceLayer(
					$"{Name}: {nameof(Debug)}",
					Debug,
					InterfaceScaleType.UI
				));
			}
		}
		private static void SetupDrawLocation(Player player, ref int x, ref int y)
		{
			if (Main.playerInventory)
			{
				//So it doesn't overlap with inventory and recipe UI
				x += 2 * INVENTORY_SIZE;
				y += 4 * INVENTORY_SIZE;

				if (player.chest != -1 || Main.npcShop != 0)
				{
					//Y offset when chest or shop open
					y += 4 * SHOP_SIZE;
				}
			}
			else
			{
				int buffsPerLine = 11;
				int lineOffset = 0;
				for (int b = buffsPerLine; b < player.buffType.Length; b += buffsPerLine)
				{
					if (player.buffType[b] > 0)
					{
						lineOffset = b / buffsPerLine;
					}
				}
				y += lineOffset * 50 + Main.buffTexture[1].Height;
				if (ModLoader.GetMod("ThoriumMod") != null)
				{
					//Bard buffs bar
					y += 16;
				}
			}
		}

		private static readonly GameInterfaceDrawMethod Debug = delegate
		{
			Player player = Main.LocalPlayer;
			int xPosition = 32;
			int yPosition = 76 + SHOP_SIZE;
			Color white = Color.White;
			Color gray = Color.Gray;
			Rectangle sourceRect;
			Rectangle destRect;
			float fade = (float)(Main.time % 120) / 120f;

			SetupDrawLocation(player, ref xPosition, ref yPosition);

			int type = ModContent.ItemType<BarbedWire>();
			Texture2D texture = Main.itemTexture[type];
			DrawAnimation drawAnim = Main.itemAnimations[type];
			if (drawAnim != null)
			{
				sourceRect = drawAnim.GetFrame(texture);
			}
			else
			{
				sourceRect = texture.Bounds;
			}
			int width = sourceRect.Width;
			int height = sourceRect.Height;
			destRect = new Rectangle(xPosition, yPosition, width, height);

			int topHeight = (int)(height * fade);
			int bottomHeight = height - topHeight;

			//draw top half 
			sourceRect.Height = topHeight;
			destRect.Height = topHeight;
			//position is top left corner so no adjustments needed
			Main.spriteBatch.Draw(texture, destRect, sourceRect, white);

			//draw bottom half
			sourceRect.Height = bottomHeight;
			destRect.Height = bottomHeight;
			sourceRect.Y += topHeight;
			destRect.Y += topHeight;

			Main.spriteBatch.Draw(texture, destRect, sourceRect, gray);

			return true;
		};
	}
}
