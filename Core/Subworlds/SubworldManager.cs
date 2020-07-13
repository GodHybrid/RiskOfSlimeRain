using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace RiskOfSlimeRain.Core.Subworlds
{
	public static class SubworldManager
	{
		public static Mod subworldLibrary = null;

		public static bool Loaded => subworldLibrary != null;

		public static SubworldMonitor Current;

		private static UnifiedRandom _miscRand;

		public static UnifiedRandom MiscRand
		{
			get
			{
				if (_miscRand == null)
				{
					_miscRand = new UnifiedRandom(DateTime.Now.Millisecond - 420);
				}
				return _miscRand;
			}
		}

		public static int TeleporterTileType => TileID.Furnaces;

		public static bool? Enter<T>() where T : Subworld
		{
			if (!Loaded) return null;
			return subworldLibrary.Call("Enter", subworldIDs[typeof(T)]) as bool?;
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

		public static bool? IsActive<T>() where T : Subworld
		{
			return IsActive(subworldIDs[typeof(T)]);
		}

		public static bool? AnyActive()
		{
			if (!Loaded) return null;
			return subworldLibrary.Call("AnyActive", RiskOfSlimeRainMod.Instance) as bool?;
		}

		/// <summary>
		/// Returns <see cref="string.Empty"/> if none active
		/// </summary>
		public static string GetActiveSubworldID()
		{
			return subworldIDs.Values.FirstOrDefault(s => IsActive(s) ?? false) ?? string.Empty;
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

		/// <summary>
		/// Maps a subworld type to its unique ID given by SubworldLibrary. Only accessed when the mod is also loaded. If mod isn't loaded, this is null
		/// </summary>
		public static Dictionary<Type, string> subworldIDs;

		public static readonly string prefix = "RiskOfSlimeRain.Core.Subworlds.";
		public static readonly string suffix = "Subworld";

		public static void RegisterSubworlds()
		{
			//Reflection shenanigans
			Type[] types = typeof(SubworldManager).Assembly.GetTypes();
			List<Type> subworldTypes = new List<Type>();
			Dictionary<string, string> loadedTypeNamespaceToName = new Dictionary<string, string>();
			foreach (var type in types)
			{
				if (!type.IsAbstract && type.IsSubclassOf(typeof(Subworld)))
				{
					if (!(type.FullName.StartsWith(prefix) && type.FullName.EndsWith(suffix)))
					{
						throw new Exception($"Error loading Subworld [{type.FullName}], it doesn't start with [{prefix}] and end with [{suffix}]");
					}
					else if (loadedTypeNamespaceToName.ContainsKey(type.Name))
					{
						throw new Exception($"Error loading Subworld [{type.FullName}], an effect with that same name already exists in [{loadedTypeNamespaceToName[type.Name]}]! Make sure to make effect names unique");
					}
					loadedTypeNamespaceToName.Add(type.Name, type.Namespace);

					subworldTypes.Add(type);
				}
			}

			foreach (var type in subworldTypes)
			{
				Subworld subworld = (Subworld)Activator.CreateInstance(type);
				string id = subworld.RegisterSelf();
				subworldIDs.Add(type, id);
			}
		}

		public static void Load()
		{
			subworldLibrary = ModLoader.GetMod("SubworldLibrary");
			if (subworldLibrary != null)
			{
				subworldIDs = new Dictionary<Type, string>();
				RegisterSubworlds();
			}
		}

		public static void Reset()
		{
			Current = null;
		}

		public static void Unload()
		{
			subworldIDs = null;
			subworldLibrary = null;
			_miscRand = null;
		}
	}
}
