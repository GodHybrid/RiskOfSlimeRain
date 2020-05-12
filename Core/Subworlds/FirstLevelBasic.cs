using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.World.Generation;

namespace RiskOfSlimeRain.Core.Subworlds
{
	public static class FirstLevelBasic
	{
		public static string id = string.Empty;

		public const int width = 300;
		public const int height = 200;

		public static void Add()
		{
			object result = SubworldManager.subworldLibrary.Call(
					"Register",
					/*Mod mod*/ RiskOfSlimeRainMod.Instance,
					/*string id*/ "First Level (Basic)",
					/*int width*/ width,
					/*int height*/ height,
					/*List<GenPass> tasks*/ ConstructSubworldGenPassList(),
					/*Action load*/ (Action)SubworldManager.GenericLoadWorld,
					/*Action unload*/ null,
					/*ModWorld modWorld*/ ModContent.GetInstance<RORWorld>()
					);

			if (result != null && result is string s)
			{
				id = s;
			}
		}

		public static void Unload()
		{
			id = string.Empty;
		}

		public static List<GenPass> ConstructSubworldGenPassList()
		{
			List<GenPass> list = new List<GenPass>
			{
				//First steps
				new PassLegacy(nameof(Adjust), Adjust, 0.01f),
				new PassLegacy(nameof(PlaceWalls), PlaceWalls, 1f),

				new PassLegacy(nameof(PlaceDirt), PlaceDirt, 1f),
				new PassLegacy(nameof(BuildPlatforms), BuildPlatforms, 1f),

				//Last steps
				new PassLegacy(nameof(SpreadGrass), SpreadGrass, 1f),
				new PassLegacy(nameof(RemoveWalls), RemoveWalls, 1f),
			};
			return list;
		}

		public static void PlaceWalls(GenerationProgress progress)
		{
			progress.Message = "Place Walls";

			WorldGen.PlaceTile(0, 0, TileID.Dirt, true);
			WorldGen.PlaceTile(1, 0, TileID.Dirt, true);
			WorldGen.PlaceTile(0, 1, TileID.Dirt, true);
			WorldGen.PlaceTile(1, 1, TileID.Dirt, true);

			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					progress.Value = (i * height + j) / (float)(width * height);
					WorldGen.PlaceWall(i, j, WallID.DiamondGemspark, true);
				}
			}

			WorldGen.KillTile(0, 0, noItem: true);
			WorldGen.KillTile(1, 0, noItem: true);
			WorldGen.KillTile(0, 1, noItem: true);
			WorldGen.KillTile(1, 1, noItem: true);
		}

		public static void RemoveWalls(GenerationProgress progress)
		{
			progress.Message = "Remove Walls";

			for (int i = 1; i < width - 1; i++)
			{
				for (int j = 1; j < height - 1; j++)
				{
					progress.Value = (i * height + j) / (float)(width * height);

					if (Main.tile[i, j].wall != WallID.DiamondGemspark) continue;

					Tile top = Main.tile[i, j - 1];
					if (!top.active() || !Main.tileSolid[top.type])
					{
						WorldGen.KillWall(i, j);
						continue;
					}
					Tile topLeft = Main.tile[i - 1, j - 1];
					if (!topLeft.active() || !Main.tileSolid[topLeft.type])
					{
						WorldGen.KillWall(i, j);
						continue;
					}
					Tile left = Main.tile[i - 1, j];
					if (!left.active() || !Main.tileSolid[left.type])
					{
						WorldGen.KillWall(i, j);
						continue;
					}
					Tile bottomLeft = Main.tile[i - 1, j + 1];
					if (!bottomLeft.active() || !Main.tileSolid[bottomLeft.type])
					{
						WorldGen.KillWall(i, j);
						continue;
					}
					Tile bottom = Main.tile[i, j + 1];
					if (!bottom.active() || !Main.tileSolid[bottom.type])
					{
						WorldGen.KillWall(i, j);
						continue;
					}
					Tile bottomRight = Main.tile[i + 1, j + 1];
					if (!bottomRight.active() || !Main.tileSolid[bottomRight.type])
					{
						WorldGen.KillWall(i, j);
						continue;
					}
					Tile right = Main.tile[i + 1, j];
					if (!right.active() || !Main.tileSolid[right.type])
					{
						WorldGen.KillWall(i, j);
						continue;
					}
					Tile topRight = Main.tile[i + 1, j - 1];
					if (!topRight.active() || !Main.tileSolid[topRight.type])
					{
						WorldGen.KillWall(i, j);
						continue;
					}
				}
			}
		}

		public static void PlaceDirt(GenerationProgress progress)
		{
			progress.Message = "Place Dirt";
			for (int i = 0; i < width; i++)
			{
				for (int j = height - 60; j < height; j++)
				{
					progress.Value = (i * height + j) / (float)(width * height);
					WorldGen.PlaceTile(i, j, TileID.Dirt, true);
				}
			}
		}

		public static void SpreadGrass(GenerationProgress progress)
		{
			progress.Message = "Spread Grass";
			for (int i = 1; i < width - 1; i++)
			{
				//bool flag = true;
				for (int j = 40; j < height - 1; j++)
				{
					Tile center = Main.tile[i, j];
					if (center.active())
					{
						Tile top = Main.tile[i, j - 1];
						if (!top.active() || !Main.tileSolid[top.type])
						{
							WorldGen.PlaceTile(i, j, TileID.Grass, true, true);
							continue;
						}
						Tile left = Main.tile[i - 1, j];
						if (!left.active())
						{
							WorldGen.PlaceTile(i, j, TileID.Grass, true, true);
							continue;
						}
						Tile bottom = Main.tile[i, j + 1];
						if (!bottom.active())
						{
							WorldGen.PlaceTile(i, j, TileID.Grass, true, true);
							continue;
						}
						Tile right = Main.tile[i + 1, j];
						if (!right.active())
						{
							WorldGen.PlaceTile(i, j, TileID.Grass, true, true);
							continue;
						}
					}
					//if (Main.tile[i, j].active())
					//{
					//	if (flag && Main.tile[i, j].type == TileID.Dirt)
					//	{
					//		try
					//		{
					//			WorldGen.SpreadGrass(i, j, TileID.Dirt, TileID.Grass, true);
					//		}
					//		catch
					//		{
					//			WorldGen.SpreadGrass(i, j, TileID.Dirt, TileID.Grass, false);
					//		}
					//	}
					//	flag = false;
					//}
					//else if (Main.tile[i, j].wall == 0)
					//{
					//	flag = true;
					//}
				}
			}
		}

		public static void Adjust(GenerationProgress progress)
		{
			progress.Message = nameof(Adjust); //Sets the text above the worldgen progress bar
			Main.worldSurface = Main.maxTilesY - 42; //Hides the underground layer just out of bounds
			Main.rockLayer = Main.maxTilesY; //Hides the cavern layer way out of bounds
		}

		public static int PlaceMetalPlatform(in int startX, in int startY, int length, bool placePillar = false, bool placeRope = false)
		{
			//2 actuated (solid top), 10 solid, 4 walls and slim
			int actuated = 2;
			int walls = 4;
			int total = length + actuated + walls;
			for (int x = 0; x < total - walls; x++)
			{
				for (int y = 1; y < 4; y++)
				{
					int i = startX + x;
					int j = startY + y;
					if (!WorldGen.InWorld(i, j)) continue;

					WorldGen.PlaceTile(i, j, TileID.Pearlwood);
					WorldGen.paintTile(i, j, SubworldManager.PaintCache[ItemID.GrayPaint]);

					if (x < actuated)
					{
						Wiring.ActuateForced(i, j);
					}
				}
			}

			//Construct strip of walls pointing to the right of the platform
			for (int x = length + actuated - 1; x < total; x++)
			{
				int i = startX + x;
				int j = startY + 2;
				if (!WorldGen.InWorld(i, j)) continue;

				WorldGen.KillWall(i, j);
				WorldGen.PlaceWall(i, j, WallID.Pearlwood, true);
				WorldGen.paintWall(i, j, SubworldManager.PaintCache[ItemID.BlackPaint]);
			}

			//Place top strip
			for (int x = 0; x < total - walls; x++)
			{
				int i = startX + x;
				if (!WorldGen.InWorld(i, startY)) continue;

				WorldGen.PlaceTile(i, startY, TileID.MartianConduitPlating);
				Main.tile[i, startY].halfBrick(true);

				int paint = ItemID.GrayPaint;
				if (x < actuated)
				{
					paint = ItemID.BlackPaint;
				}
				WorldGen.paintTile(i, startY, SubworldManager.PaintCache[paint]);
			}

			//Construct pillar below the platform towards the neatest tile
			if (placePillar)
			{
				const int bottomPlatform = 4;
				const int tileAmount = 2;
				int pillarY = bottomPlatform;
				int pillarX;
				int pillarStartX = length / 2 - 1;
				int pillarWidth = 3;
				int pillarStartY = startY + pillarY;

				Tile tile = Main.tile[pillarStartX, pillarStartY];
				while (startY + pillarY < height && !tile.active())
				{
					int start = pillarStartX;
					int end = pillarStartX + pillarWidth;

					bool placeTiles = pillarY < bottomPlatform + tileAmount;

					if (placeTiles)
					{
						start--;
						end++;
					}

					for (pillarX = start; pillarX < end; pillarX++)
					{
						int i = startX + pillarX;
						int j = startY + pillarY;
						if (!WorldGen.InWorld(i, j)) continue;

						int paint = ItemID.GrayPaint;
						if (pillarX == end - 1)
						{
							paint = ItemID.BlackPaint;
						}

						if (placeTiles)
						{
							WorldGen.PlaceTile(i, j, TileID.TinPlating);
							WorldGen.paintTile(i, j, SubworldManager.PaintCache[paint]);
							Wiring.ActuateForced(i, j);
						}
						else
						{
							WorldGen.KillWall(i, j);
							WorldGen.PlaceWall(i, j, WallID.TinPlating, true);
							WorldGen.paintWall(i, j, SubworldManager.PaintCache[paint]);
						}
					}

					if (placeTiles)
					{
						pillarY++;
					}
					tile = Main.tile[startX + pillarX, startY + pillarY];
					if (!placeTiles)
					{
						pillarY++;
					}
				}
			}

			//Construct rope below the platform towards the neatest tile
			if (placeRope)
			{
				int ropeX = length + actuated;
				int ropeY = 0;

				int dirtX = startY + ropeX;
				int dirtY = -1;

				//Place a dirt block above the rope so it has something to attach to
				if (startY + ropeY > 0)
				{
					dirtY = startY + ropeY - 1;
					WorldGen.PlaceTile(dirtX, dirtY, TileID.Dirt);
				}

				Tile tile = Main.tile[startX + ropeX, startY + ropeY];
				while (startY + ropeY < height && !tile.active())
				{
					int i = startX + ropeX;
					int j = startY + ropeY;
					if (!WorldGen.InWorld(i, j)) continue;

					WorldGen.PlaceTile(i, j, TileID.Rope);

					ropeY++;
					tile = Main.tile[i, startY + ropeY];
				}

				//Remove the dirt block
				if (dirtY != -1)
				{
					WorldGen.KillTile(dirtX, dirtY);
				}

				//Remove 4 ropes at the bottom
				for (int bottomY = -3; bottomY < 1; bottomY++)
				{
					int i = startX + ropeX;
					int j = startY + ropeY + bottomY;
					if (!WorldGen.InWorld(i, j)) continue;

					tile = Main.tile[i, j];
					if (tile.type == TileID.Rope)
					{
						WorldGen.KillTile(i, j);
					}
				}
			}

			return startX + total;
		}

		public static void BuildPlatforms(GenerationProgress progress)
		{
			progress.Message = "Build Platforms";

			int nextX = Main.spawnTileX - 6;
			int y = Main.spawnTileY + 7;
			nextX = PlaceMetalPlatform(nextX, y, 10, placeRope: true);
			nextX = PlaceMetalPlatform(nextX, y, 10, placePillar: true);
			PlaceMetalPlatform(nextX, y, 10);
		}
	}
}
