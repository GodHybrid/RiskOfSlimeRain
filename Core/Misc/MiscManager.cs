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

		/// <summary>
		/// Types of NPCs which are immune to all buffs
		/// </summary>
		private static int[] isBuffImmune;

		/// <summary>
		/// Checks if an NPC is a vanilla boss piece/minion
		/// </summary>
		public static bool IsBossPiece(NPC npc)
		{
			if (IsChild(npc, out NPC parent))
			{
				if (parent.boss)
				{
					return true;
				}
			}

			switch (npc.type)
			{
				case NPCID.Creeper:
				case NPCID.EaterofWorldsBody:
				case NPCID.EaterofWorldsTail:
				case NPCID.Golem:
				case NPCID.GolemFistLeft:
				case NPCID.GolemFistRight:
				case NPCID.MartianSaucerCannon:
				case NPCID.MartianSaucerTurret:
				case NPCID.MoonLordCore:
				case NPCID.MoonLordHand:
				case NPCID.MoonLordHead:
				case NPCID.MoonLordFreeEye:
				case NPCID.PlanterasHook:
				case NPCID.PlanterasTentacle:
				case NPCID.PrimeCannon:
				case NPCID.PrimeLaser:
				case NPCID.PrimeSaw:
				case NPCID.PrimeVice:
				case NPCID.SkeletronHead:
				case NPCID.TheDestroyerBody:
				case NPCID.TheDestroyerTail:
				case NPCID.TheHungry:
				case NPCID.TheHungryII:
				case NPCID.Probe:
					return true;
				default:
					return false;
			}
		}

		/// <summary>
		/// Check if this NPC is tied to the healthpool of another NPC
		/// </summary>
		public static bool IsChild(NPC npc, out NPC parent)
		{
			bool child = npc.realLife != npc.whoAmI && npc.realLife >= 0 && npc.realLife <= Main.maxNPCs;
			parent = child ? Main.npc[npc.realLife] : null;
			return child;
		}

		public static void Load()
		{
			LoadNPCCache();
		}

		public static void Unload()
		{
			isModdedWormBodyOrTail = null;
			isBuffImmune = null;
		}

		/// <summary>
		/// Checks if given NPC is a worm body or tail
		/// </summary>
		public static bool IsWormBodyOrTail(NPC npc)
		{
			//dontCountMe is the general check, EaterofWorlds stuff is because it's special, and if that fails, check modded array
			return npc.dontCountMe || npc.type == NPCID.EaterofWorldsTail || npc.type == NPCID.EaterofWorldsBody || Array.BinarySearch(isModdedWormBodyOrTail, npc.type) >= 0 /* || npc.realLife != -1*/;
		}

		/// <summary>
		/// Checks if given NPC is immune to all buffs
		/// </summary>
		public static bool IsBuffImmune(NPC npc)
		{
			return Array.BinarySearch(isBuffImmune, npc.type) >= 0;
		}

		/// <summary>
		/// Loads various data related to NPCs
		/// </summary>
		public static void LoadNPCCache()
		{
			List<int> wormList = new List<int>();
			List<int> buffList = new List<int>();

			for (int i = 0; i < NPCLoader.NPCCount; i++)
			{
				try
				{
					bool buffImmune = true;
					NPC npc = new NPC();
					npc.SetDefaults(i);
					for (int j = 0; j < npc.buffImmune.Length; j++)
					{
						if (!npc.buffImmune[j])
						{
							buffImmune = false;
							break;
						}
					}
					if (buffImmune)
					{
						buffList.Add(i);
					}

					//Modded only
					if (i >= Main.maxNPCTypes)
					{
						ModNPC modNPC = npc.modNPC;
						if (modNPC != null)
						{
							string name = modNPC.GetType().Name;
							if (name.EndsWith("Body") || name.EndsWith("Tail"))
							{
								//Fills isModdedWormBodyOrTail with types of modded NPCs which names are ending with Body or Tail (indicating they are part of something)
								wormList.Add(i);
							}
						}
					}
				}
				catch
				{

				}
			}

			isModdedWormBodyOrTail = wormList.ToArray();
			Array.Sort(isModdedWormBodyOrTail);

			isBuffImmune = buffList.ToArray();
			Array.Sort(isBuffImmune);
		}
	}
}
