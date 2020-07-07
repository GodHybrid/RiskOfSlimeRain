using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.Utilities;

namespace RiskOfSlimeRain.Core.Subworlds
{
	//Contains worldgen methods
	public abstract partial class Subworld
	{
		public int PlacePlatform(in int startX, in int startY, int length, int topType, int type, int beamType, byte paint = 0, float topCut = -1f, float bottomCut = -1f)
		{
			for (int x = 0; x < length; x++)
			{
				for (int y = 0; y < 2; y++)
				{
					int i = startX + x;
					int j = startY + y;
					if (!WorldGen.InWorld(i, j)) continue;

					//Example
					//0: Wood
					//1: Living wood
					bool placed = WorldGen.PlaceTile(i, j, y == 0 ? topType : type);
					if (placed && paint > 0)
					{
						WorldGen.paintTile(i, j, paint);
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

				PlaceBeam(i, j, beamType, 0, topCut, bottomCut);
			}

			return startX + length;
		}

		public void PlaceBeam(in int startX, in int startY, int type, byte paint = 0, float topCut = -1f, float bottomCut = -1f)
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
			while (startY + beamY < Main.maxTilesY && (firstWall || !tile.active()))
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

		public int PlaceMetalPlatform(in int startX, in int startY, int length, bool actuate = true, bool placeWalls = true, bool placePillar = false, bool placeRope = false)
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
				while (startY + pillarY < Main.maxTilesY && !tile.active())
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

		public void PlaceRope(in int startX, in int startY, int type = TileID.Rope, int cutOffBottom = 4)
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
			while (startY + ropeY < Main.maxTilesY && !tile.active())
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

		public void PlaceLadder(in int startX, in int startY, in int height, int ladderType, byte ladderPaint = 0)
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
								if (oldType == topType)
								{
									wallType = topWallType;
									paintType = topWallPaint;
								}
								else/* if (oldType == TerrainType)*/
								{
									wallType = terrainWallType;
									paintType = terrainWallPaint;
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

					WorldGen.PlaceTile(i, j, ladderType, true);
					if (ladderPaint > 0)
					{
						WorldGen.paintTile(i, j, ladderPaint);
					}
				}

				if (placedDirt)
				{
					WorldGen.KillTile(dirtX, dirtY);
				}
			}
		}

		/// <summary>
		/// Returns the coordinates of a random solid tile that has other solid tiles left & (right + 1) of it, and air above it. Returns Point.Zero if not found
		/// </summary>
		public Point GetBottomCenterOfAirPocket(UnifiedRandom rand, in int left, in int right, in int top, in int maxTries = 1000)
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

			int minX = 42;
			int maxX = Main.maxTilesX - 42;
			int minY = 42;
			int maxY = Main.maxTilesY - 42;

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

		/// <summary>
		/// Creates a box of the specified tile. Leave it -1 to create a hole instead
		/// </summary>
		public void BuildRectangle(in int startX, in int startY, in int width, in int height, int type = -1, byte paint = 0, Action<int, int> onAction = null)
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
