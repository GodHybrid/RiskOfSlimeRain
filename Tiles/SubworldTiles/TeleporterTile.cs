using System;
using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Items.Placeable;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.ID;
using RiskOfSlimeRain.Core.Subworlds;
using RiskOfSlimeRain.Items;
using Microsoft.Xna.Framework.Graphics;

namespace RiskOfSlimeRain.Tiles.SubworldTiles
{
	public class TeleporterTile : DroppableTile<TeleporterItem>
	{
		//"Hitbox"
		public const int width = 9;
		public const int height = 5;

		public const int centerX = 4;
		public const int centerY = 3;

		public override void SetDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = true;
			Main.tileWaterDeath[Type] = true;
			TileID.Sets.HasOutlines[Type] = true;
			disableSmartCursor = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style5x4);
			TileObjectData.newTile.Width = width;
			TileObjectData.newTile.Height = height;
			TileObjectData.newTile.Origin = new Point16(centerX, height - 1);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16, 16 };
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(Type);
			ModTranslation name = CreateMapEntryName();
			dustType = -1;
			soundType = -1;
			name.SetDefault("Teleporter");
			AddMapEntry(new Color(162, 184, 185), name);
		}

		public override bool HasSmartInteract()
		{
			return true;
		}

		public override void MouseOver(int i, int j)
		{
			Player player = Main.LocalPlayer;
			player.mouseInterface = true;
			player.noThrow = 2;
			player.showItemIcon = true;
			player.showItemIcon2 = ModContent.ItemType<NoTextureItem>();

			if (SubworldManager.Monitor == null) return;

			if (!SubworldManager.Monitor.TeleporterActivated)
			{
				player.showItemIconText = "Right Click to activate teleporter. Are you ready?";
			}
			else if (SubworldManager.Monitor.TeleporterReady)
			{
				//"Right Click to teleport to the next level"
				player.showItemIconText = "Right Click to teleport back to the main world";
			}
			else
			{
				player.showItemIconText = "STAY ALIVE!";
			}
		}

		public override bool NewRightClick(int i, int j)
		{
			if (SubworldManager.Monitor == null) return false;

			if (!SubworldManager.Monitor.TeleporterActivated)
			{
				Main.PlaySound(SoundID.Mech, i * 16, j * 16, 0);
				SubworldManager.Monitor.ActivateTeleporter(true);
				return true;
			}
			else if (SubworldManager.Monitor.TeleporterReady)
			{
				SubworldManager.Monitor.InitiateTeleportSequence(true);
				return true;
			}

			return false;
		}

		public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
		{
			Tile tile = Main.tile[i, j];
			Texture2D texture;
			if (Main.canDrawColorTile(i, j))
			{
				texture = Main.tileAltTexture[Type, tile.color()];
			}
			else
			{
				texture = Main.tileTexture[Type];
			}
			Vector2 zero = new Vector2(Main.offScreenRange);
			if (Main.drawToScreen)
			{
				zero = Vector2.Zero;
			}
			//int height = 16;
			int height = tile.frameY == texture.Height - 18 ? 18 : 16;
			Color color = Lighting.GetColor(i, j);
			Vector2 pos = new Vector2(i * 16 - (int)Main.screenPosition.X, j * 16 - (int)Main.screenPosition.Y) + zero;
			pos.Y += 2;
			Rectangle frame = new Rectangle(tile.frameX, tile.frameY, 16, height);
			spriteBatch.Draw(texture, pos, frame, color, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);

			Color outlineColor = Color.Transparent;
			if (TileID.Sets.HasOutlines[Type])
			{
				int average = (color.R + color.G + color.B) / 3;
				bool selected = false;
				bool mouseContains = Main.SmartInteractTileCoords.Contains(new Point(i, j));
				if (mouseContains && Collision.InTileBounds(i, j, Main.TileInteractionLX, Main.TileInteractionLY, Main.TileInteractionHX, Main.TileInteractionHY))
				{
					selected = true;
				}

				texture = Main.highlightMaskTexture[Type];
				if (selected && average > 10)
				{
					outlineColor = new Color(255, 255, 255 / 3);
				}
				else
				{
					var current = SubworldManager.Monitor;
					if (current != null)
					{
						bool drawGreen = current.TeleporterActivated && current.TeleportReadyTimerDone;
						if (drawGreen)
						{
							outlineColor = new Color(192, 255, 192);
						}
						else //drawRed
						{
							outlineColor = new Color(214, 41, 16);
						}
					}
				}
				outlineColor.MultiplyRGBA(color);

				spriteBatch.Draw(texture, pos, frame, outlineColor, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
			}

			return false;
		}

		//TODO debug only
		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			if (frameX == 0 && frameY == 0)
			{
				Item.NewItem((i + centerX) * 16, (j + centerY) * 16, 16, 16, ItemType);
			}
		}
	}
}
