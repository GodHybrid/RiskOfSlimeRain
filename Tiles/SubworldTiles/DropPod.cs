using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RiskOfSlimeRain.Items.Placeable;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.ID;

namespace RiskOfSlimeRain.Tiles.SubworldTiles
{
	public class DropPod : ModTile
	{
		const int frameCount = 2;
		const int spriteHeight = 220;

		public override void SetDefaults()
		{
			//6x6
			Main.tileFrameImportant[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style6x3);
			TileObjectData.newTile.Height = 6;
			TileObjectData.newTile.Origin = new Point16(3, 5);
			TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16, 16, 16, 18 };
			TileObjectData.addTile(Type);
			AddMapEntry(new Color(162, 184, 185));
			animationFrameHeight = spriteHeight / frameCount;
			disableSmartCursor = true;
		}

		public override void AnimateTile(ref int frame, ref int frameCounter)
		{
			frameCounter++;
			if (frameCounter >= (frame == 0 ? 300 : 8))
			{
				frameCounter = 0;
				frame++;
				if (frame >= frameCount)
				{
					frame = 0;
				}
			}
		}
	}
}
