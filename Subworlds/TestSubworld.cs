using SubworldLibrary;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Subworlds
{
	public class TestSubworld : Subworld
	{
		//public override int width => 2 * 42 + 2 * 60 - 1;

		//public override int height => 2 * 42 + 2 * 31 - 1;
		public override int width => 600;

		public override int height => 400;

		public override ModWorld modWorld => null;

		public override SubworldGenPass[] tasks => new SubworldGenPass[]
		{
			new SubworldGenPass("cock", 1f, progress =>
			{
				progress.Message = "cock"; //Sets the text above the worldgen progress bar
				Main.worldSurface = Main.maxTilesY - 42; //Hides the underground layer just out of bounds
				Main.rockLayer = Main.maxTilesY; //Hides the cavern layer way out of bounds
			}),
			new SubworldGenPass("cock2", 1f, progress =>
			{
				progress.Message = "cock2";

				for (int i = -1; i < 2; i++)
				{
					WorldGen.PlaceTile(Main.spawnTileX - i,  Main.spawnTileY + 2, TileID.Dirt, true, true);
				}

				for (int i = 0; i < width; i++)
				{
					for (int j = 0; j < height; j++)
					{
						if (i < 42 || i >= width - 43 || j <= 41 || j >= height - 43)
						{
							WorldGen.PlaceTile(i, j, TileID.LihzahrdBrick, true, true);
						}
					}
				}
			})
		};

		public override void Load()
		{
			Main.dayTime = true;
			Main.time = 27000;
			Main.worldRate = 0;
		}
	}
}
