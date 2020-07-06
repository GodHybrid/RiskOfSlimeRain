using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Core.Subworlds
{
	public static class SubworldManager
	{
		public static Mod subworldLibrary = null;

		public static bool Loaded => subworldLibrary != null;

		public static SubworldMonitor Current;

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

		public static Dictionary<Type, Subworld> subworlds;

		public static void Load()
		{
			subworldLibrary = ModLoader.GetMod("SubworldLibrary");
			if (subworldLibrary != null)
			{
				subworlds = new Dictionary<Type, Subworld>();

				FirstLevelBasic.Add();
			}
		}

		public static void Reset()
		{
			Current = null;
		}

		public static void Unload()
		{
			subworldLibrary = null;
			FirstLevelBasic.Unload();
		}

		public static void GenericLoadWorld()
		{
			Main.dayTime = true;
			Main.time = 27000;
		}
	}
}
