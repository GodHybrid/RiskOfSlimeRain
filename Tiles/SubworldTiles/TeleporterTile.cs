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

			if (SubworldManager.Current == null) return;

			if (!SubworldManager.Current.TeleporterActivated)
			{
				player.showItemIconText = "Right Click to activate teleporter. Are you ready?";
			}
			else if (SubworldManager.Current.TeleporterReady)
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
			if (SubworldManager.Current == null) return false;

			if (!SubworldManager.Current.TeleporterActivated)
			{
				Main.PlaySound(SoundID.Mech, i * 16, j * 16, 0);
				SubworldManager.Current.ActivateTeleporter(true);
				return true;
			}
			else if (SubworldManager.Current.TeleporterReady)
			{
				SubworldManager.Current.InitiateTeleportSequence(true);
				return true;
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
