using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace RiskOfSlimeRain.Tiles.SubworldTiles
{
	public class FirstLevelSand : ModTile
	{
		public override void SetDefaults()
		{
			Main.tileSolid[Type] = true;
			Main.tileBrick[Type] = true;
			Main.tileMergeDirt[Type] = true;
			Main.tileBlockLight[Type] = true;
			Main.tileSand[Type] = true;
			TileID.Sets.TouchDamageSands[Type] = 15;
			TileID.Sets.Conversion.Sand[Type] = true; // Allows Clentaminator solutions to convert this tile to their respective Sand tiles.
			TileID.Sets.ForAdvancedCollision.ForSandshark[Type] = true; // Allows Sandshark enemies to "swim" in this sand.
			TileID.Sets.Falling[Type] = true;
			AddMapEntry(new Color(200, 200, 200));
		}
	}
}
