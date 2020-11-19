using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace RiskOfSlimeRain.Tiles.SubworldTiles
{
	public class DriedLakeGrass : ModTile
	{
		public override void SetDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.Origin = new Point16(0, 1);
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.newTile.RandomStyleRange = 8;
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.addTile(Type);
			dustType = -1;
			soundType = -1;
		}
	}
}
