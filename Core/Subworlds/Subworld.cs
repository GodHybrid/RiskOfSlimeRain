using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Tiles.SubworldTiles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.World.Generation;

namespace RiskOfSlimeRain.Core.Subworlds
{
	/// <summary>
	/// This class is used to define the terrain of a subworld. Instances of this class don't exist, they just get used for SubworldLibrary to register
	/// </summary>
	public abstract partial class Subworld
	{
		public readonly string displayName;

		public readonly int width;

		public readonly int height;

		//The "main material"
		public readonly int terrainType;
		public readonly byte terrainPaint;
		public readonly int terrainWallType;
		public readonly byte terrainWallPaint;
		//The "floor"
		public readonly int topType;
		public readonly byte topPaint;
		public readonly int topWallType;
		public readonly byte topWallPaint;

		/// <summary>
		/// Subworlds have to have a parameterless constructor calling base on this one
		/// </summary>
		public Subworld(string displayName, int width, int height, int terrainType, byte terrainPaint, int terrainWallType, byte terrainWallPaint, int topType, byte topPaint, int topWallType, byte topWallPaint)
		{
			this.displayName = displayName;
			this.width = width;
			this.height = height;

			this.terrainType = terrainType;
			this.terrainPaint = terrainPaint;
			this.terrainWallType = terrainWallType;
			this.terrainWallPaint = terrainWallPaint;

			this.topType = topType;
			this.topPaint = topPaint;
			this.topWallType = topWallType;
			this.topPaint = topWallPaint;
		}

		public virtual void LoadWorld()
		{
			Main.dayTime = true;
			Main.time = 27000;
			for (int i = 0; i < Main.maxClouds; i++)
			{
				Main.cloud[i] = new Cloud();
			}
		}

		public virtual List<GenPass> Generation()
		{
			return new List<GenPass>();
		}

		/// <summary>
		/// Does the Mod.Call, and if successful, returns its assigned ID, otherwise <see cref="string.Empty"/>
		/// </summary>
		public string RegisterSelf()
		{
			string ret = string.Empty;
			object result = null;

			try
			{
				result = SubworldManager.subworldLibrary.Call(
						"Register",
						/*Mod mod*/ RiskOfSlimeRainMod.Instance,
						/*string id*/ displayName,
						/*int width*/ width,
						/*int height*/ height,
						/*List<GenPass> tasks*/ Generation(),
						/*Action load*/ (Action)LoadWorld,
						/*Action unload*/ null,
						/*ModWorld modWorld*/ ModContent.GetInstance<RORWorld>()
						);
			}
			catch
			{

			}

			if (result != null && result is string id)
			{
				return id;
			}

			return ret;
		}
	}

	public class DriedLakeSubworld : Subworld
	{
		public DriedLakeSubworld() : base("Dried Lake - Ground Zero", 360, 600, TileID.GrayStucco, PaintID.Black, WallID.Gray, PaintID.Black, ModContent.TileType<FirstLevelSand>(), PaintID.Yellow, WallID.YellowStucco, 0)
		{

		}

		public override List<GenPass> Generation()
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
				new PassLegacy(nameof(PlaceTeleporter), PlaceTeleporter, 1f),
			};
			return list;
		}

		public void PlaceWalls(GenerationProgress progress)
		{
			progress.Message = "Place Walls";

			WorldGen.PlaceTile(0, 0, TileID.Dirt, true);
			WorldGen.PlaceTile(1, 0, TileID.Dirt, true);
			WorldGen.PlaceTile(0, 1, TileID.Dirt, true);
			WorldGen.PlaceTile(1, 1, TileID.Dirt, true);

			for (int i = 0; i < Main.maxTilesX; i++)
			{
				for (int j = 0; j < Main.maxTilesY; j++)
				{
					progress.Value = (i * Main.maxTilesY + j) / (float)(Main.maxTilesX * Main.maxTilesY);
					WorldGen.PlaceWall(i, j, WallID.DiamondGemspark, true);
				}
			}

			WorldGen.KillTile(0, 0, noItem: true);
			WorldGen.KillTile(1, 0, noItem: true);
			WorldGen.KillTile(0, 1, noItem: true);
			WorldGen.KillTile(1, 1, noItem: true);
		}

		public void RemoveWalls(GenerationProgress progress)
		{
			progress.Message = "Remove Walls";

			for (int i = 1; i < Main.maxTilesX - 1; i++)
			{
				for (int j = 1; j < Main.maxTilesY - 1; j++)
				{
					progress.Value = (i * Main.maxTilesY + j) / (float)(Main.maxTilesX * Main.maxTilesY);

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

		public void PlaceTopBorder(GenerationProgress progress)
		{
			progress.Message = "Place Top Border";
			for (int i = 0; i < Main.maxTilesX; i++)
			{
				for (int j = 0; j < topBorder; j++)
				{
					progress.Value = (i * topBorder + j) / (float)(Main.maxTilesX * topBorder);
					WorldGen.PlaceTile(i, j, TileID.DiamondGemspark, true);
					WorldGen.paintTile(i, j, PaintID.Black);
				}
			}
		}

		public void PlaceTerrain(GenerationProgress progress)
		{
			progress.Message = "Place Terrain";
			progress.Value = 0f;
			//Floor
			BuildRectangle(0, GroundLevel, width, GroundLevel, terrainType, terrainPaint);

			//Left wall
			BuildRectangle(0, topBorder, leftWallWidth, height - topBorder, terrainType, terrainPaint);

			//Uneven-ness of left wall
			BuildRectangle(leftWallWidth - 4, TopLeftTBaseY, 4, 16);

			//Right wall
			BuildRectangle(RightWallX, RightPlatformY, rightWallWidth, rightPlatformHeight, terrainType, terrainPaint);

			//Right wall (smaller one)
			BuildRectangle(RightWallX + 20, RightPlatformY - 100, rightWallWidth, 100, terrainType, terrainPaint);

			//Platform extending ouf of the wall
			BuildRectangle(RightPlatformX, RightPlatformY, rightWallWidth + rightPlatformWidth, 6, terrainType, terrainPaint);

			//Rectangle on the right side of the platform
			BuildRectangle(RightWallX + 2, RightPlatformY - 12, rightWallWidth - 2, 12, terrainType, terrainPaint);

			//Uneven-ness of right wall
			BuildRectangle(RightWallX - 4, RightPlatformY + 6, 4, 6, terrainType, terrainPaint);
			BuildRectangle(RightWallX, RightPlatformY + 30, 2, 28);
			BuildRectangle(RightWallX, RightPlatformY + 40, 4, 8);

			//T-shape for center
			BuildRectangle(TopLeftTBaseX, TopLeftTBaseY, baseTWidth, baseTHeight, terrainType, terrainPaint);
			BuildRectangle(TopLeftTSideX, TopLeftTBaseY, TSideWidth, topLeftTSideHeight, terrainType, terrainPaint);

			//Area the center is on
			BuildRectangle(TopLeftTSideX + 1, CenterY, TSideWidth - 2, CenterbaseHeight, terrainType, terrainPaint);

			//Clear center
			BuildRectangle(TopLeftTBaseX, CenterY - 6, baseTWidth, 6);

			//Holes left and right of center
			//Staircase on left/right
			int holeWidth = 10;
			int holeHeight = 6;

			int leftHoleTopLeftX = CenterX - baseTWidth / 2 - holeWidth;
			int rightHoleTopRightX = CenterX + baseTWidth / 2 + holeWidth - 1;

			BuildRectangle(leftHoleTopLeftX, CenterY, holeWidth, holeHeight);
			BuildRectangle(CenterX + baseTWidth / 2, CenterY, holeWidth - 1, holeHeight);

			for (int x = 0; x < holeHeight; x++)
			{
				for (int y = 0; y < holeHeight; y++)
				{
					if (x > y) continue;

					int i = leftHoleTopLeftX + x;
					int j = CenterY + y;

					BuildRectangle(i, j, 1, 1, terrainType, terrainPaint);
				}
			}

			for (int x = 0; x < holeHeight; x++)
			{
				for (int y = 0; y < holeHeight; y++)
				{
					if (holeHeight - x > y) continue;

					int i = rightHoleTopRightX - holeHeight + x;
					int j = CenterY + y;

					BuildRectangle(i, j, 1, 1, terrainType, terrainPaint);
				}
			}
		}

		public void CoverTerrainWithTop(GenerationProgress progress)
		{
			progress.Message = "Cover Terrain with Top";
			for (int i = 0; i < Main.maxTilesX; i++)
			{
				for (int j = topBorder; j < Main.maxTilesY - 1; j++)
				{
					progress.Value = (i * Main.maxTilesY + (j - topBorder)) / (float)(Main.maxTilesX * Main.maxTilesY);

					Tile tile = Main.tile[i, j];
					if (tile.active() && tile.type == terrainType)
					{
						Tile top = Main.tile[i, j - 1];
						if (!top.active())
						{
							WorldGen.PlaceTile(i, j - 1, topType, true);
							if (topPaint > 0)
							{
								WorldGen.paintTile(i, j - 1, topPaint);
							}
						}
						Tile top2 = Main.tile[i, j - 2];
						if (!top2.active())
						{
							WorldGen.PlaceTile(i, j - 2, topType, true);
							if (topPaint > 0)
							{
								WorldGen.paintTile(i, j - 2, topPaint);
							}
						}
					}
				}
			}
		}

		//Unused
		//public void SpreadGrass(GenerationProgress progress)
		//{
		//	progress.Message = "Spread Grass";
		//	for (int i = 1; i < Main.maxTilesX - 1; i++)
		//	{
		//		for (int j = topBorder; j < Main.maxTilesY - 1; j++)
		//		{
		//			progress.Value = (i * Main.maxTilesY + j) / (float)(Main.maxTilesX * Main.maxTilesY);
		//			Tile center = Main.tile[i, j];
		//			if (center.active() && center.type == TileID.Dirt)
		//			{
		//				bool placedTile = false;
		//				for (int a = -1; a < 2; a++)
		//				{
		//					for (int b = -1; b < 2; b++)
		//					{
		//						if (a != 0 && b != 0)
		//						{
		//							Tile tile = Main.tile[i + a, j + b];
		//							if (!tile.active() || !Main.tileSolid[tile.type])
		//							{
		//								WorldGen.PlaceTile(i, j, TileID.Grass, true, true);
		//								placedTile = true;
		//							}
		//						}
		//						if (placedTile) break;
		//					}
		//					if (placedTile) break;
		//				}
		//				if (placedTile) continue;
		//			}
		//		}
		//	}
		//}

		public void Adjust(GenerationProgress progress)
		{
			progress.Message = "Adjust"; //Sets the text above the worldgen progress bar
			Main.worldSurface = Main.maxTilesY - 42; //Hides the underground layer just out of bounds
			Main.rockLayer = Main.maxTilesY; //Hides the cavern layer way out of bounds

			//Arbitrary spawn location away from the visible borders
			Main.spawnTileX = 2;
			Main.spawnTileY = 3;
		}

		public void PlaceTeleporter(GenerationProgress progress)
		{
			progress.Message = "Place Teleporter";

			int tries = 0;
			const int maxTries = 1000;
			Point point = Point.Zero;
			while (tries < maxTries)
			{
				point = GetBottomCenterOfAirPocket(SubworldManager.MiscRand, 3, 4, 4);

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

		public void BuildLadders(GenerationProgress progress)
		{
			progress.Message = "Build Ladders";
			progress.Value = 0f;

			int x = TopLeftTSideX + TSideWidth - 10;
			int y = TopLeftTBaseY - 2;
			int height = baseTHeight;
			PlaceLadder(x, y, height, TileID.SilkRope, PaintID.Yellow);
			x = TopLeftTSideX + 10 - 2;
			PlaceLadder(x, y, height, TileID.SilkRope, PaintID.Yellow);
		}

		public void SetSpawn(GenerationProgress progress)
		{
			progress.Message = "Set Spawn";

			//Fallback spawn
			Main.spawnTileX = CenterX - 1;
			Main.spawnTileY = CenterY - 2; //Because sand is 2 tiles thick

			Point point = GetBottomCenterOfAirPocket(SubworldManager.MiscRand, 2, 3, 4);

			if (point != Point.Zero)
			{
				Main.spawnTileX = point.X - 1;
				Main.spawnTileY = point.Y;
			}
		}

		public void BuildMetalPlatforms(GenerationProgress progress)
		{
			progress.Message = "Build Metal Platforms";
			progress.Value = 0f;

			int x = TopLeftTSideX + 20;
			int y = RightMiddleMetalPlatformY;
			x = PlaceMetalPlatform(x, y, 10, placePillar: true);
			x = PlaceMetalPlatform(x, y, 10, placeRope: true);
			PlaceMetalPlatform(x, y, 40, placeWalls: false);

			x = TopLeftTSideX + 4;
			y = RightPlatformY - 2;
			x = PlaceMetalPlatform(x, y, 10, placeRope: true);
			PlaceMetalPlatform(x, y, 10, placePillar: true, placeWalls: false);
		}

		public void BuildWoodenPlatforms(GenerationProgress progress)
		{
			progress.Message = "Build Wooden Platforms";
			progress.Value = 0f;

			int highPlatformLength = 38;

			//Right side
			//The one in the right hole

			int x = TopLeftTSideX + TSideWidth - 2;
			int y = CenterY - 2;
			int length = Main.maxTilesX - rightWallWidth - x;
			x = PlacePlatform(x, y, 4, PlatformTopType, PlatformType, PlatformBeamWallType, PlatformPaint);
			PlaceRope(x, y);
			PlacePlatform(x + 2, y, length - 6, PlatformTopType, PlatformType, PlatformBeamWallType, PlatformPaint);

			////
			//Left side
			////

			//Low left platform
			x = leftWallWidth;
			y = LowPlatformY;
			PlacePlatform(x, y, 50, PlatformTopType, PlatformType, PlatformBeamWallType, PlatformPaint);
			//No rope

			//Low right platforms
			int platformLength = 8;
			int gap = 4;
			int safeDistanceFromLeftTSide = gap + 3;
			x = TopLeftTSideX - 2 * platformLength - safeDistanceFromLeftTSide;
			x = PlacePlatform(x, y, platformLength, PlatformTopType, PlatformType, PlatformBeamWallType, PlatformPaint, 0f, 0f);
			PlacePlatform(x + gap, y, platformLength, PlatformTopType, PlatformType, PlatformBeamWallType, PlatformPaint);
			//No ropes

			//Middle left platform
			x = leftWallWidth - 4; //because of the dent
			y = MiddlePlatformY;
			int leftMiddleRopeX = PlacePlatform(x, y, 24, PlatformTopType, PlatformType, PlatformBeamWallType, PlatformPaint);

			//Middle right platforms
			x = leftMiddleRopeX + highPlatformLength / 2;
			int middleMiddleRopeX = x - 1;
			//this one is made up of 3 
			int middleLength = 24;
			x = PlacePlatform(x, y, middleLength / 3, PlatformTopType, PlatformType, PlatformBeamWallType, PlatformPaint, 0.1f, 1f);
			x = PlacePlatform(x, y, middleLength / 3, PlatformTopType, PlatformType, PlatformBeamWallType, PlatformPaint, 0f, 0f);
			x = PlacePlatform(x, y, middleLength / 3, PlatformTopType, PlatformType, PlatformBeamWallType, PlatformPaint, 0.1f, 1f);
			//
			x = PlacePlatform(x + gap, y, 8, PlatformTopType, PlatformType, PlatformBeamWallType, PlatformPaint, 0f, 0f);
			int rightBound = TopLeftTSideX - safeDistanceFromLeftTSide;
			int lastLength = rightBound - x;
			x += gap;
			int rightMiddleRopeX = x - 1;
			PlacePlatform(x, y, lastLength, PlatformTopType, PlatformType, PlatformBeamWallType, PlatformPaint);

			//High platform
			x = HighPlatformX;
			y = HighPlatformY;
			platformLength = highPlatformLength;
			int guaranteedBeamSection = 6;
			int leftHighRopeX = x - 1;
			x = PlacePlatform(x, y, platformLength / 2, PlatformTopType, PlatformType, PlatformBeamWallType, PlatformPaint, 0.1f, 1f);
			x = PlacePlatform(x, y, guaranteedBeamSection, PlatformTopType, PlatformType, PlatformBeamWallType, PlatformPaint, 0f, 0f);
			int rightHighRopeX = PlacePlatform(x, y, platformLength / 2 - guaranteedBeamSection, PlatformTopType, PlatformType, PlatformBeamWallType, PlatformPaint, 0.1f, 1f);

			//Place ropes where necessary
			PlaceRope(leftMiddleRopeX, MiddlePlatformY);
			PlaceRope(middleMiddleRopeX, MiddlePlatformY);
			PlaceRope(rightMiddleRopeX, MiddlePlatformY);
			PlaceRope(leftHighRopeX, HighPlatformY);
			PlaceRope(rightHighRopeX, HighPlatformY);

			//x = PlaceWoodenPlatform(x + 4, y + 10, 7);
			//PlaceWoodenPlatform(x + 4, y + 20, 16);
		}

		public const int topBorder = 420;

		private int CenterbaseHeight = 14;
		private int GroundDepth = 56;
		public int GroundLevel => height - GroundDepth;

		public int CenterX => width - 160;
		public int CenterY => GroundLevel - CenterbaseHeight;

		public int rightPlatformWidth = 54;
		public int leftWallWidth = 42 + 20;
		public int rightWallWidth = 100;
		public int rightPlatformHeight = 140;
		public int baseTHeight = 30;

		public int RightWallX => width - rightWallWidth;
		public int RightPlatformX => RightWallX - rightPlatformWidth;
		public int RightPlatformY => height - rightPlatformHeight;

		public int TopLeftTBaseY => CenterY - baseTHeight;

		public int RightMiddleMetalPlatformY => (RightPlatformY + TopLeftTBaseY) / 2 - 2;


		public int baseTWidth = 8;
		public int topLeftTSideHeight = 6;
		public int sideTWidth = 26;
		public int TopLeftTBaseX => CenterX - baseTWidth / 2;
		public int TopLeftTSideX => TopLeftTBaseX - sideTWidth;
		public int TSideWidth => 2 * sideTWidth + baseTWidth;

		public int PlatformTopType = TileID.WoodBlock;
		public int PlatformType = TileID.LivingWood;
		public byte PlatformPaint = 0/*ItemID.YellowPaint*/;

		public int PlatformBeamWallType = WallID.WoodenFence;

		public int LowPlatformY => CenterY - 4;
		public int MiddlePlatformY => TopLeftTBaseY + topLeftTSideHeight + 2;
		public int HighPlatformX => leftWallWidth + 12;
		public int HighPlatformY => RightMiddleMetalPlatformY;
	}
}
