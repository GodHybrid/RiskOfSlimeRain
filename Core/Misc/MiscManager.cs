using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Core.Misc
{
	/// <summary>
	/// Miscellaneous data stuffs
	/// </summary>
	public static class MiscManager
	{
		/// <summary>
		/// Types of modded NPCs which names are ending with Body or Tail
		/// </summary>
		private static int[] isModdedWormBodyOrTail;

		public static void Load()
		{
			LoadWormList();
		}

		public static void Unload()
		{
			isModdedWormBodyOrTail = null;
		}

		/// <summary>
		/// Checks if given NPC is a worm body or tail
		/// </summary>
		public static bool IsWormBodyOrTail(NPC npc)
		{
			return npc.dontCountMe || npc.type == NPCID.EaterofWorldsTail || npc.type == NPCID.EaterofWorldsBody || Array.BinarySearch(isModdedWormBodyOrTail, npc.type) >= 0 /* || npc.realLife != -1*/;
		}

		/// <summary>
		/// Fills isModdedWormBodyOrTail with types of modded NPCs which names are ending with Body or Tail (indicating they are part of something)
		/// </summary>
		public static void LoadWormList()
		{
			List<int> tempList = new List<int>();

			for (int i = Main.maxNPCTypes; i < NPCLoader.NPCCount; i++)
			{
				ModNPC modNPC = NPCLoader.GetNPC(i);
				string name = modNPC.GetType().Name;
				if (modNPC != null && (name.EndsWith("Body") || name.EndsWith("Tail")))
				{
					tempList.Add(modNPC.npc.type);
				}
			}

			isModdedWormBodyOrTail = tempList.ToArray();
			Array.Sort(isModdedWormBodyOrTail);
		}
	}
}
