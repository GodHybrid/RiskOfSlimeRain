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
		const int frameCount = 7;
		const int spriteHeight = 518;

		public override void SetDefaults()
		{
			//7x4
			Main.tileFrameImportant[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style5x4);
			TileObjectData.newTile.Width = 7;
			TileObjectData.newTile.Origin = new Point16(3, 3);
			TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16, 18 };
			TileObjectData.addTile(Type);
			AddMapEntry(new Color(162, 184, 185));
			animationFrameHeight = spriteHeight / frameCount;
			disableSmartCursor = true;
		}

		public override void AnimateTile(ref int frame, ref int frameCounter)
		{
			frameCounter++;
			if (frame == 0)
			{
				if (frameCounter >= 300)
				{
					frameCounter = 0;
					frame++;
				}
			}
			else
			{
				if (frameCounter >= 8)
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
}
