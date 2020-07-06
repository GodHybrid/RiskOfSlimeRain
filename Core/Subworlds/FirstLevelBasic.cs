using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Tiles.SubworldTiles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;
using Terraria.World.Generation;

namespace RiskOfSlimeRain.Core.Subworlds
{
	public static class FirstLevelBasic
	{
		public static string id = string.Empty;

		public const int width = 360;
		public const int height = 600;
		public const int topBorder = 420;

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

				new PassLegacy(nameof(PlaceTopBorder), PlaceTopBorder, 1f),

				new PassLegacy(nameof(PlaceTerrain), PlaceTerrain, 1f),
				new PassLegacy(nameof(CoverTerrainWithTop), CoverTerrainWithTop, 1f),

				new PassLegacy(nameof(BuildMetalPlatforms), BuildMetalPlatforms, 1f),
				new PassLegacy(nameof(BuildWoodenPlatforms), BuildWoodenPlatforms, 1f),

				//Last steps
				new PassLegacy(nameof(BuildLadders), BuildLadders, 1f),
				new PassLegacy(nameof(RemoveWalls), RemoveWalls, 1f),
				new PassLegacy(nameof(SetSpawn), SetSpawn, 1f),
				new PassLegacy(nameof(SetTeleporter), SetTeleporter, 1f),
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

					Tile tile = Main.tile[i, j];

					//Only remove gemspark walls
					if (tile.wall != WallID.DiamondGemspark) continue;

					//Remove gemspark walls if the tile is a gemspark too
					if (tile.active() && tile.type == TileID.DiamondGemspark)
					{
						WorldGen.KillWall(i, j);
						continue;
					}

					bool killedWall = false;
					for (int a = -1; a < 2; a++)
					{
						for (int b = -1; b < 2; b++)
						{
							if (a != 0 && b != 0)
							{
								Tile t = Main.tile[i + a, j + b];
								if (!t.active() || !Main.tileSolid[t.type])
								{
									WorldGen.KillWall(i, j);
									killedWall = true;
								}
							}
							if (killedWall) break;
						}
						if (killedWall) break;
					}
					if (killedWall) continue;
				}
			}
		}

		public static void PlaceTopBorder(GenerationProgress progress)
		{
			progress.Message = "Place Top Border";
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < topBorder; j++)
				{
					progress.Value = (i * topBorder + j) / (float)(width * topBorder);
					WorldGen.PlaceTile(i, j, TileID.DiamondGemspark, true);
					WorldGen.paintTile(i, j, PaintID.Black);
				}
			}
		}

		public static int TerrainType = TileID.GrayStucco;
		public static byte TerrainPaint = PaintID.Black;

		public static int TerrainWallType = WallID.Gray;
		public static byte TerrainWallPaint = PaintID.Black;

		private static int _TopType = -2;
		public static int TopType
		{
			get
			{
				if (_TopType == -2)
				{
					_TopType = ModContent.TileType<FirstLevelSand>();
				}
				return _TopType;
			}
		}
		public static byte TopPaint = PaintID.Yellow;
		public static int TopWallType = WallID.YellowStucco;
		public static byte TopWallPaint = 0;

		public static int PlatformBeamWallType = WallID.WoodenFence;

		private const int centerbaseHeight = 14;
		private const int groundDepth = 56;
		public const int groundLevel = height - groundDepth;

		public const int centerX = width - 160;
		public const int centerY = groundLevel - centerbaseHeight;

		public const int rightPlatformWidth = 54;
		public const int leftWallWidth = 42 + 20;
		public const int rightWallWidth = 100;
		public const int rightPlatformHeight = 140;
		public const int baseTHeight = 30;

		public static int rightWallX = width - rightWallWidth;
		public static int rightPlatformX = rightWallX - rightPlatformWidth;
		public static int rightPlatformY = height - rightPlatformHeight;

		public static int topLeftTBaseY = centerY - baseTHeight;

		public static int rightMiddleMetalPlatformY = (rightPlatformY + topLeftTBaseY) / 2 - 2;


		public const int baseTWidth = 8;
		public const int topLeftTSideHeight = 6;
		public const int sideTWidth = 26;
		public static int topLeftTBaseX = centerX - baseTWidth / 2;
		public static int topLeftTSideX = topLeftTBaseX - sideTWidth;
		public static int TSideWidth = 2 * sideTWidth + baseTWidth;

		public static void PlaceTerrain(GenerationProgress progress)
		{
			progress.Message = "Place Terrain";
			progress.Value = 0f;
			//Floor
			BuildRectangle(0, groundLevel, width, groundLevel, TerrainType, TerrainPaint);

			//Left wall
			BuildRectangle(0, topBorder, leftWallWidth, height - topBorder, TerrainType, TerrainPaint);

			//Uneven-ness of left wall
			BuildRectangle(leftWallWidth - 4, topLeftTBaseY, 4, 16);

			//Right wall
			BuildRectangle(rightWallX, rightPlatformY, rightWallWidth, rightPlatformHeight, TerrainType, TerrainPaint);

			//Right wall (smaller one)
			BuildRectangle(rightWallX + 20, rightPlatformY - 100, rightWallWidth, 100, TerrainType, TerrainPaint);

			//Platform extending ouf of the wall
			BuildRectangle(rightPlatformX, rightPlatformY, rightWallWidth + rightPlatformWidth, 6, TerrainType, TerrainPaint);

			//Rectangle on the right side of the platform
			BuildRectangle(rightWallX + 2, rightPlatformY - 12, rightWallWidth - 2, 12, TerrainType, TerrainPaint);

			//Uneven-ness of right wall
			BuildRectangle(rightWallX - 4, rightPlatformY + 6, 4, 6, TerrainType, TerrainPaint);
			BuildRectangle(rightWallX, rightPlatformY + 30, 2, 28);
			BuildRectangle(rightWallX, rightPlatformY + 40, 4, 8);

			//T-shape for center
			BuildRectangle(topLeftTBaseX, topLeftTBaseY, baseTWidth, baseTHeight, TerrainType, TerrainPaint);
			BuildRectangle(topLeftTSideX, topLeftTBaseY, TSideWidth, topLeftTSideHeight, TerrainType, TerrainPaint);

			//Area the center is on
			BuildRectangle(topLeftTSideX + 1, centerY, TSideWidth - 2, centerbaseHeight, TerrainType, TerrainPaint);

			//Clear center
			BuildRectangle(topLeftTBaseX, centerY - 6, baseTWidth, 6);

			//Holes left and right of center
			//Staircase on left/right
			int holeWidth = 10;
			int holeHeight = 6;

			int leftHoleTopLeftX = centerX - baseTWidth / 2 - holeWidth;
			int rightHoleTopRightX = centerX + baseTWidth / 2 + holeWidth - 1;

			BuildRectangle(leftHoleTopLeftX, centerY, holeWidth, holeHeight);
			BuildRectangle(centerX + baseTWidth / 2, centerY, holeWidth - 1, holeHeight);

			for (int x = 0; x < holeHeight; x++)
			{
				for (int y = 0; y < holeHeight; y++)
				{
					if (x > y) continue;

					int i = leftHoleTopLeftX + x;
					int j = centerY + y;

					BuildRectangle(i, j, 1, 1, TerrainType, TerrainPaint);
				}
			}

			for (int x = 0; x < holeHeight; x++)
			{
				for (int y = 0; y < holeHeight; y++)
				{
					if (holeHeight - x > y) continue;

					int i = rightHoleTopRightX - holeHeight + x;
					int j = centerY + y;

					BuildRectangle(i, j, 1, 1, TerrainType, TerrainPaint);
				}
			}
		}

		public static void CoverTerrainWithTop(GenerationProgress progress)
		{
			progress.Message = "Cover Terrain with Top";
			for (int i = 0; i < width; i++)
			{
				for (int j = topBorder; j < height - 1; j++)
				{
					progress.Value = (i * height + (j - topBorder)) / (float)(width * height);

					Tile tile = Main.tile[i, j];
					if (tile.active() && tile.type == TerrainType)
					{
						Tile top = Main.tile[i, j - 1];
						if (!top.active())
						{
							WorldGen.PlaceTile(i, j - 1, TopType, true);
							if (TopPaint > 0)
							{
								WorldGen.paintTile(i, j - 1, TopPaint);
							}
						}
						Tile top2 = Main.tile[i, j - 2];
						if (!top2.active())
						{
							WorldGen.PlaceTile(i, j - 2, TopType, true);
							if (TopPaint > 0)
							{
								WorldGen.paintTile(i, j - 2, TopPaint);
							}
						}
					}
				}
			}
		}

		//Unused
		public static void SpreadGrass(GenerationProgress progress)
		{
			progress.Message = "Spread Grass";
			for (int i = 1; i < width - 1; i++)
			{
				for (int j = topBorder; j < height - 1; j++)
				{
					progress.Value = (i * height + j) / (float)(width * height);
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
				}
			}
		}

		public static void Adjust(GenerationProgress progress)
		{
			progress.Message = nameof(Adjust); //Sets the text above the worldgen progress bar
			Main.worldSurface = Main.maxTilesY - 42; //Hides the underground layer just out of bounds
			Main.rockLayer = Main.maxTilesY; //Hides the cavern layer way out of bounds

			//Arbitrary spawn location away from the visible borders
			Main.spawnTileX = 2;
			Main.spawnTileY = 3;
		}

		//TODO change
		public static UnifiedRandom miscRand = new UnifiedRandom(DateTime.Now.Millisecond - 420);

		public static void SetTeleporter(GenerationProgress progress)
		{
			progress.Message = nameof(SetTeleporter); //Sets the text above the worldgen progress bar

			int tries = 0;
			const int maxTries = 1000;
			Point point = Point.Zero;
			while (tries < maxTries)
			{
				point = GetBottomCenterOfAirPocket(miscRand, 3, 4, 4);

				const int radiusX = 30;
				const int radiusY = 20;
				if (point.X > Main.spawnTileX - radiusX && point.X < Main.spawnTileX + radiusX)
				{
					continue;
				}
				else if (point.Y > Main.spawnTileY - radiusY && point.Y < Main.spawnTileY + radiusY)
				{
					continue;
				}
				break;
			}

			if (point == Point.Zero)
			{
				point = new Point(Main.spawnTileX, Main.spawnTileY);
			}

			//x is the middle coordinate, y the bottom
			WorldGen.Place3x2(point.X, point.Y - 1, TileID.Furnaces);
		}

		/// <summary>
		/// Returns the coordinates of a random solid tile that has other solid tiles left & (right + 1) of it, and air above it. Returns Point.Zero if not found
		/// </summary>
		private static Point GetBottomCenterOfAirPocket(UnifiedRandom rand, in int left, in int right, in int top, in int maxTries = 1000)
		{
			//Randomizer technique:
			/*
			 * Pick random x, y
			 * Check if its air
			 * Raycast down until hits solid
			 * Check if atleast topY tiles of air above it
			 * Check if left and right of it theres total leftX + rightX solid tiles
			 * If not, retry
			 */

			Point point = Point.Zero;

			int tries = 0;

			bool noAir = true;

			const int minX = 42;
			const int maxX = width - 42;
			const int minY = 42;
			const int maxY = height - 42;

			for (int x = minX; x < maxX; x++)
			{
				for (int y = minY; y < maxY; y++)
				{
					Tile tile = Main.tile[x, y];
					if (!tile.active()) noAir = false;
					if (!noAir) break;
				}
				if (!noAir) break;
			}

			if (noAir) return point; //Continue with fallback

			while (tries < maxTries)
			{
				tries++;
				int x = rand.Next(minX, maxX);
				int y = rand.Next(minY, maxY);

				Tile tile = Main.tile[x, y];
				int validX = x;
				int validY = y;
				if (!tile.active())
				{
					int yBelow = validY - 1;
					tile = Main.tile[x, yBelow];
					while (yBelow < maxY && !tile.active())
					{
						yBelow++;
						tile = Main.tile[x, yBelow];
					}

					if (yBelow >= maxY) continue;

					//Now the tile is a solid
					validY = yBelow;

					//Check tiles left and right
					bool notValidSides = false;
					for (x = validX - left; x < validX + 1 + right; x++)
					{
						tile = Main.tile[x, validY];
						if (!tile.active() || tile.slope() != 0 || tile.halfBrick() || !Main.tileSolid[tile.type])
						{
							notValidSides = true;
						}
						if (notValidSides) break;
						else
						{
							for (y = validY - top; y < validY; y++)
							{
								tile = Main.tile[x, y];
								if (tile.active()) notValidSides = true;
								if (notValidSides) break;
								//Check air tiles above the tile
							}
						}
					}

					if (notValidSides) continue;

					point = new Point(validX, validY);
					break;
				}
			}

			return point;
		}

		public static void SetSpawn(GenerationProgress progress)
		{
			progress.Message = nameof(SetSpawn); //Sets the text above the worldgen progress bar

			//Fallback spawn
			Main.spawnTileX = centerX - 1;
			Main.spawnTileY = centerY - 2; //Because sand is 2 tiles thick

			Point point = GetBottomCenterOfAirPocket(miscRand, 2, 3, 4);

			if (point != Point.Zero)
			{
				Main.spawnTileX = point.X - 1;
				Main.spawnTileY = point.Y;
			}
		}

		public static void BuildMetalPlatforms(GenerationProgress progress)
		{
			progress.Message = "Build Metal Platforms";
			progress.Value = 0f;

			int x = topLeftTSideX + 20;
			int y = rightMiddleMetalPlatformY;
			x = PlaceMetalPlatform(x, y, 10, placePillar: true);
			x = PlaceMetalPlatform(x, y, 10, placeRope: true);
			PlaceMetalPlatform(x, y, 40, placeWalls: false);

			x = topLeftTSideX + 4;
			y = rightPlatformY - 2;
			x = PlaceMetalPlatform(x, y, 10, placeRope: true);
			PlaceMetalPlatform(x, y, 10, placePillar: true, placeWalls: false);
		}

		public static int PlatformTopType = TileID.WoodBlock;
		public static int PlatformType = TileID.LivingWood;
		public static byte PlatformPaint = 0/*ItemID.YellowPaint*/;

		public static int LowPlatformY = centerY - 4;
		public static int MiddlePlatformY = topLeftTBaseY + topLeftTSideHeight + 2;
		public static int HighPlatformX = leftWallWidth + 12;
		public static int HighPlatformY = rightMiddleMetalPlatformY;

		public static void BuildWoodenPlatforms(GenerationProgress progress)
		{
			progress.Message = "Build Wooden Platforms";
			progress.Value = 0f;

			int highPlatformLength = 38;

			//Right side
			//The one in the right hole

			int x = topLeftTSideX + TSideWidth - 2;
			int y = centerY - 2;
			int length = width - rightWallWidth - x;
			x = PlaceWoodenPlatform(x, y, 4);
			PlaceRope(x, y);
			PlaceWoodenPlatform(x + 2, y, length - 6);

			////
			//Left side
			////

			//Low left platform
			x = leftWallWidth;
			y = LowPlatformY;
			PlaceWoodenPlatform(x, y, 50);
			//No rope

			//Low right platforms
			int platformLength = 8;
			int gap = 4;
			int safeDistanceFromLeftTSide = gap + 3;
			x = topLeftTSideX - 2 * platformLength - safeDistanceFromLeftTSide;
			x = PlaceWoodenPlatform(x, y, platformLength, 0f, 0f);
			PlaceWoodenPlatform(x + gap, y, platformLength);
			//No ropes

			//Middle left platform
			x = leftWallWidth - 4; //because of the dent
			y = MiddlePlatformY;
			int leftMiddleRopeX = PlaceWoodenPlatform(x, y, 24);

			//Middle right platforms
			x = leftMiddleRopeX + highPlatformLength / 2;
			int middleMiddleRopeX = x - 1;
			//this one is made up of 3 
			int middleLength = 24;
			x = PlaceWoodenPlatform(x, y, middleLength / 3, 0.1f, 1f);
			x = PlaceWoodenPlatform(x, y, middleLength / 3, 0f, 0f);
			x = PlaceWoodenPlatform(x, y, middleLength / 3, 0.1f, 1f);
			//
			x = PlaceWoodenPlatform(x + gap, y, 8, 0f, 0f);
			int rightBound = topLeftTSideX - safeDistanceFromLeftTSide;
			int lastLength = rightBound - x;
			x += gap;
			int rightMiddleRopeX = x - 1;
			PlaceWoodenPlatform(x, y, lastLength);

			//High platform
			x = HighPlatformX;
			y = HighPlatformY;
			platformLength = highPlatformLength;
			int guaranteedBeamSection = 6;
			int leftHighRopeX = x - 1;
			x = PlaceWoodenPlatform(x, y, platformLength / 2, 0.1f, 1f);
			x = PlaceWoodenPlatform(x, y, guaranteedBeamSection, 0f, 0f);
			int rightHighRopeX = PlaceWoodenPlatform(x, y, platformLength / 2 - guaranteedBeamSection, 0.1f, 1f);

			//Place ropes where necessary
			PlaceRope(leftMiddleRopeX, MiddlePlatformY);
			PlaceRope(middleMiddleRopeX, MiddlePlatformY);
			PlaceRope(rightMiddleRopeX, MiddlePlatformY);
			PlaceRope(leftHighRopeX, HighPlatformY);
			PlaceRope(rightHighRopeX, HighPlatformY);

			//x = PlaceWoodenPlatform(x + 4, y + 10, 7);
			//PlaceWoodenPlatform(x + 4, y + 20, 16);
		}

		public static int PlaceWoodenPlatform(in int startX, in int startY, int length, float topCut = -1f, float bottomCut = -1f)
		{
			for (int x = 0; x < length; x++)
			{
				for (int y = 0; y < 2; y++)
				{
					int i = startX + x;
					int j = startY + y;
					if (!WorldGen.InWorld(i, j)) continue;

					//0: Wood
					//1: Living wood
					if (y == 0)
					{
						bool placed = WorldGen.PlaceTile(i, j, PlatformTopType);
						if (placed && PlatformPaint > 0)
						{
							WorldGen.paintTile(i, j, PlatformPaint);
						}
					}
					else
					{
						bool placed = WorldGen.PlaceTile(i, j, PlatformType);
						if (placed && PlatformPaint > 0)
						{
							WorldGen.paintTile(i, j, PlatformPaint);
						}
					}
				}
			}

			//Beams
			for (int x = 0; x < length; x++)
			{
				int i = startX + x;

				if (i % 5 != 0) continue;

				int j = startY + 1;

				if (!WorldGen.InWorld(i, j)) continue;
				Tile left = Main.tile[i - 1, j];
				if (!left.active()) continue;
				Tile center = Main.tile[i, j];
				if (!center.active()) continue;
				Tile right = Main.tile[i + 1, j];
				if (!right.active()) continue;

				PlaceBeam(i, j, PlatformBeamWallType, 0, topCut, bottomCut);
			}

			return startX + length;
		}

		public static void PlaceBeam(in int startX, in int startY, int type, byte paint = 0, float topCut = -1f, float bottomCut = -1f)
		{
			int beamX = 0;
			int beamY = 0;
			int i = startX + beamX;
			int j = startY + beamY;
			if (!WorldGen.InWorld(i, j))
			{
				throw new ArgumentOutOfRangeException("starting arguments are out of range");
			}
			Tile tile = Main.tile[i, j];
			bool firstWall = true;
			while (startY + beamY < height && (firstWall || !tile.active()))
			{
				firstWall = false;
				if (!WorldGen.InWorld(i, j)) continue;

				WorldGen.KillWall(i, j);
				WorldGen.PlaceWall(i, j, type, true);
				if (paint > 0)
				{
					WorldGen.paintWall(i, j, paint);
				}

				beamY++;
				j = startY + beamY;
				tile = Main.tile[i, j];
			}

			if (topCut <= 0f)
			{
				topCut = WorldGen.genRand.NextFloat(0f, 0.8f);
			}
			if (bottomCut < 0f)
			{
				bottomCut = WorldGen.genRand.NextFloat(0.3f, 1f);
			}

			if (bottomCut <= topCut) return; //No cut

			beamY--;
			int length = beamY;
			int topCutY = Math.Max(1, (int)(length * topCut));
			int bottomCutY = Math.Max(1, (int)(length * bottomCut));

			for (int y = topCutY; y <= bottomCutY; y++)
			{
				j = startY + y;
				if (!WorldGen.InWorld(i, j)) continue;
				tile = Main.tile[i, j];
				if (tile.wall == type)
				{
					WorldGen.KillWall(i, j);
				}
			}
		}

		public static int PlaceMetalPlatform(in int startX, in int startY, int length, bool actuate = true, bool placeWalls = true, bool placePillar = false, bool placeRope = false)
		{
			const int minLength = 5;
			if (length < minLength)
			{
				length = minLength;
			}
			//2 actuated (solid top), 10 solid, 4 walls and slim
			int actuated = 2;
			int walls = 4;
			int total = length;
			if (actuate)
			{
				total += actuated;
			}
			for (int x = 0; x < total; x++)
			{
				for (int y = 1; y < 4; y++)
				{
					int i = startX + x;
					int j = startY + y;
					if (!WorldGen.InWorld(i, j)) continue;

					bool placed = WorldGen.PlaceTile(i, j, TileID.Pearlwood);
					if (placed)
					{
						WorldGen.paintTile(i, j, PaintID.Gray);

						if (actuate && x < actuated)
						{
							Wiring.ActuateForced(i, j);
						}
					}
				}
			}

			if (placeWalls)
			{
				//Construct strip of walls pointing to the right of the platform
				for (int x = total - 1; x < total + walls; x++)
				{
					int i = startX + x;
					int j = startY + 2;
					if (!WorldGen.InWorld(i, j)) continue;

					WorldGen.KillWall(i, j);
					WorldGen.PlaceWall(i, j, WallID.Pearlwood, true);
					WorldGen.paintWall(i, j, PaintID.Black);
				}
			}

			//Place top strip
			for (int x = 0; x < total; x++)
			{
				int i = startX + x;
				if (!WorldGen.InWorld(i, startY)) continue;

				bool placed = WorldGen.PlaceTile(i, startY, TileID.MartianConduitPlating);
				if (placed)
				{
					Main.tile[i, startY].halfBrick(true);

					byte paint = PaintID.Gray;
					if (actuate && x < actuated)
					{
						paint = PaintID.Black;
					}
					WorldGen.paintTile(i, startY, paint);
				}
			}

			if (placeWalls)
			{
				total += walls;
			}

			//Construct pillar below the platform towards the neatest tile
			if (placePillar)
			{
				const int bottomPlatform = 4;
				const int tileAmount = 2;
				int pillarY = bottomPlatform;
				int pillarX;
				int pillarStartX = minLength;

				const int pillarWidth = 3;

				if (pillarStartX >= length - pillarWidth)
				{
					pillarStartX -= pillarWidth - 1;
				}

				int pillarStartXTile = startX + pillarStartX;
				int pillarStartYTile = startY + pillarY;

				if (!WorldGen.InWorld(pillarStartXTile, pillarStartYTile)) return startX + total;

				Tile tile = Main.tile[pillarStartXTile, pillarStartYTile];
				while (startY + pillarY < height && !tile.active())
				{
					int start = pillarStartX;
					int end = pillarStartX + pillarWidth;

					//For the first two layers, place tiles, and also wider
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

						byte paint = PaintID.Gray;
						if (pillarX == end - 1)
						{
							paint = PaintID.Black;
						}

						if (placeTiles)
						{
							WorldGen.PlaceTile(i, j, TileID.TinPlating);
							WorldGen.paintTile(i, j, paint);
							Wiring.ActuateForced(i, j);
						}
						else
						{
							WorldGen.KillWall(i, j);
							WorldGen.PlaceWall(i, j, WallID.TinPlating, true);
							WorldGen.paintWall(i, j, paint);
						}
					}

					if (placeTiles)
					{
						pillarY++;
					}

					if (!WorldGen.InWorld(startX + pillarX, startY + pillarY)) break;

					tile = Main.tile[startX + pillarX, startY + pillarY];

					if (!placeTiles)
					{
						pillarY++;
					}
				}
			}

			//Construct rope below the platform towards the neatest tile
			//Only place rope if walls also placed (otherwise no space to maneuver)
			if (placeRope && placeWalls)
			{
				int ropeX = startX + length;
				if (actuate)
				{
					ropeX += actuated;
				}
				PlaceRope(ropeX, startY);
			}

			return startX + total;
		}

		public static void PlaceRope(in int startX, in int startY, int type = TileID.Rope, int cutOffBottom = 4)
		{
			int ropeX = 0;
			int ropeY = 0;

			int dirtX = startX + ropeX;
			int dirtY = startY + ropeY - 1;
			bool placedDirt = false;

			//Place a dirt block above the rope so it has something to attach to
			if (startY + ropeY > 0)
			{
				placedDirt = WorldGen.PlaceTile(dirtX, dirtY, TileID.Dirt);
			}

			Tile tile = Main.tile[startX + ropeX, startY + ropeY];
			while (startY + ropeY < height && !tile.active())
			{
				int i = startX + ropeX;
				int j = startY + ropeY;
				if (!WorldGen.InWorld(i, j)) continue;

				WorldGen.PlaceTile(i, j, type);

				ropeY++;
				tile = Main.tile[i, startY + ropeY];
			}

			//Remove the dirt block
			if (placedDirt)
			{
				WorldGen.KillTile(dirtX, dirtY);
			}

			//Remove 4 ropes at the bottom
			for (int bottomY = -cutOffBottom; bottomY < 0; bottomY++)
			{
				int i = startX + ropeX;
				int j = startY + ropeY + bottomY;
				if (!WorldGen.InWorld(i, j)) continue;

				tile = Main.tile[i, j];
				if (tile.type == type)
				{
					WorldGen.KillTile(i, j);
				}
			}
		}

		public static void BuildLadders(GenerationProgress progress)
		{
			progress.Message = "Build Ladders";
			progress.Value = 0f;

			int x = topLeftTSideX + TSideWidth - 10;
			int y = topLeftTBaseY - 2;
			int height = baseTHeight;
			PlaceLadder(x, y, height, TileID.SilkRope, PaintID.Yellow);
			x = topLeftTSideX + 10 - 2;
			PlaceLadder(x, y, height, TileID.SilkRope, PaintID.Yellow);
		}

		public static void PlaceLadder(in int startX, in int startY, in int height, int type, byte paint = 0)
		{
			//Start at the top, break tiles if there are any in the way. Starting tile doesn't get walls behind it

			for (int x = 0; x < 2; x++)
			{
				int dirtX = startX + x;
				int dirtY = startY - 1;
				bool placedDirt = false;

				//Place a dirt block above the rope so it has something to attach to
				if (dirtY >= 0)
				{
					placedDirt = WorldGen.PlaceTile(dirtX, dirtY, TileID.Dirt);
				}

				for (int y = 0; y < height; y++)
				{
					int i = startX + x;
					int j = startY + y;

					if (!WorldGen.InWorld(i, j)) continue;

					Tile tile = Main.tile[i, j];

					//Replace tiles with walls on the way
					int oldType = -1;
					if (tile.active())
					{
						oldType = tile.type;
						WorldGen.KillTile(i, j);
					}
					if (oldType > -1 && y > 0)
					{
						if (WorldGen.InWorld(i, j + 1))
						{
							Tile below = Main.tile[i, j + 1];
							if (below.active())
							{
								int wallType;
								byte paintType;
								if (oldType == TopType)
								{
									wallType = TopWallType;
									paintType = TopWallPaint;
								}
								else/* if (oldType == TerrainType)*/
								{
									wallType = TerrainWallType;
									paintType = TerrainWallPaint;
								}
								WorldGen.KillWall(i, j);
								WorldGen.PlaceWall(i, j, wallType, true);
								if (paintType > 0)
								{
									WorldGen.paintWall(i, j, paintType);
								}
							}
						}
					}

					WorldGen.PlaceTile(i, j, type, true);
					if (paint > 0)
					{
						WorldGen.paintTile(i, j, paint);
					}
				}

				if (placedDirt)
				{
					WorldGen.KillTile(dirtX, dirtY);
				}
			}
		}

		/// <summary>
		/// Creates a box of the specified tile. Leave it -1 to create a hole instead
		/// </summary>
		public static void BuildRectangle(in int startX, in int startY, in int width, in int height, int type = -1, byte paint = 0, Action<int, int> onAction = null)
		{
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					int i = startX + x;
					int j = startY + y;
					if (!WorldGen.InWorld(i, j)) continue;

					if (type == -1)
					{
						WorldGen.KillTile(i, j);
					}
					else
					{
						WorldGen.PlaceTile(i, j, type, true);
						if (paint > 0)
						{
							WorldGen.paintTile(i, j, paint);
						}
					}
					onAction?.Invoke(i, j);
				}
			}
		}
	}
}
