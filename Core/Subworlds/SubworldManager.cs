using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.World.Generation;

namespace RiskOfSlimeRain.Core.Subworlds
{
	public static class SubworldManager
	{
		public static string firstWorld = string.Empty;

		public static Mod subworldLibrary = null;

		public static bool Loaded => subworldLibrary != null;

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

		public static bool? IsActive(string id)
		{
			if (!Loaded) return null;
			return subworldLibrary.Call("IsActive", id) as bool?;
		}

		public static bool? AnyActive(Mod mod)
		{
			if (!Loaded) return null;
			return subworldLibrary.Call("AnyActive", mod) as bool?;
		}

		/*
		 * 	if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				bool? result = SubworldManager.Enter(SubworldManager.firstWorld);
				return result ?? false;
			}
		 */

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
					/*string id*/ "example",
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

		public static void Unload()
		{
			subworldLibrary = null;
			firstWorld = string.Empty;
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
				new PassLegacy(nameof(Adjust), Adjust, 0.01f),
				new PassLegacy(nameof(BuildBox), BuildBox, 1f)
			};
			return list;
		}

		public static void Adjust(GenerationProgress progress)
		{
			progress.Message = nameof(Adjust); //Sets the text above the worldgen progress bar
			Main.worldSurface = Main.maxTilesY - 42; //Hides the underground layer just out of bounds
			Main.rockLayer = Main.maxTilesY; //Hides the cavern layer way out of bounds
		}

		public static void BuildBox(GenerationProgress progress)
		{
			progress.Message = "Build Box";

			for (int i = -1; i < 2; i++)
			{
				WorldGen.PlaceTile(Main.spawnTileX - i, Main.spawnTileY + 2, TileID.Dirt, true, true);
			}

			for (int i = 0; i < 600; i++)
			{
				for (int j = 0; j < 400; j++)
				{
					progress.Value = (i * 400f + j) / (600 * 400);
					if (i < 42 || i >= 600 - 43 || j < 42 || j >= 400 - 43)
					{
						WorldGen.PlaceTile(i, j, TileID.LihzahrdBrick, true, true);
					}
				}
			}
		}
	}
}
