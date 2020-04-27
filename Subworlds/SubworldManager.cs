using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.World.Generation;

namespace RiskOfSlimeRain.Subworlds
{
	public static class SubworldManager
	{
		public static string firstWorld = string.Empty;

		public static Mod subworldLibrary = null;

		public static bool Loaded => subworldLibrary != null;

		/*
		 * new SubworldGenPass("cock", 1f, progress =>
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
		 */

		public static bool? Enter(string id)
		{
			if (!Loaded) return null;
			return subworldLibrary.Call("Enter", id) as bool?;
		}

		public static bool? Exit()
		{
			if (!Loaded) return null;
			return subworldLibrary.Call("Exit") as bool?;
		}


		/*
		 "Register"
		 Mod mod,
		 string id,
		 int width,
		 int height,
		 List<GenPass> tasks,
		 Action load = null, Action unload = null, ModWorld modWorld = null, bool saveSubworld = false, bool disablePlayerSaving = false, bool saveModData = false, bool noWorldUpdate = true, UserInterface loadingUI = null, UIState loadingUIState = null, UIState votingUI = null, ushort votingDuration = 1800, Action onVotedFor = null
		*/
		public static void Load()
		{
			subworldLibrary = ModLoader.GetMod("SubworldLibrary");
			if (subworldLibrary != null)
			{
				object result = subworldLibrary.Call(
					"Register",
					/*Mod mod*/ RiskOfSlimeRainMod.Instance,
					/*string id*/ "cock",
					/*int width*/ 600,
					/*int height*/ 400,
					/*List<GenPass> tasks*/ ConstructSubworldGenPassList(),
					/*Action load*/ (Action)GenericLoadWorld,
					/*Action unload*/ null,
					/*ModWorld modWorld*/ ModContent.GetInstance<RORWorld>()
					);

				if (result != null && result is string id)
				{
					firstWorld = id;
				}
			}
		}

		public static void GenericLoadWorld()
		{
			Main.dayTime = true;
			Main.time = 27000;
		}

		public static List<GenPass> ConstructSubworldGenPassList()
		{
			List<GenPass> list = new List<GenPass>
			{
				// First pass
				new PassLegacy("cock",
				delegate (GenerationProgress progress)
				{
					progress.Message = "cock"; //Sets the text above the worldgen progress bar
					Main.worldSurface = Main.maxTilesY; //Hides the underground layer just out of bounds
					//Main.worldSurface = Main.maxTilesY - 42; //Hides the underground layer just out of bounds
					Main.rockLayer = Main.maxTilesY; //Hides the cavern layer way out of bounds
				},
				1f),
				// Second pass
				new PassLegacy("cock2",
				delegate (GenerationProgress progress)
				{
					progress.Message = "cock2";

					for (int i = -1; i < 2; i++)
					{
						WorldGen.PlaceTile(Main.spawnTileX - i,  Main.spawnTileY + 2, TileID.Dirt, true, true);
					}

					for (int i = 0; i < 600; i++)
					{
						for (int j = 0; j < 400; j++)
						{
							progress.Value = (i * 400f + j) / (600 * 400);
							if (i < 42 || i >= 600 - 43 || j <= 41 || j >= 400 - 43)
							{
								WorldGen.PlaceTile(i, j, TileID.LihzahrdBrick, true, true);
							}
						}
					}
				},
				1f)
			};

			return list;
		}

		public static void Unload()
		{
			subworldLibrary = null;
			firstWorld = string.Empty;
		}
	}
}
