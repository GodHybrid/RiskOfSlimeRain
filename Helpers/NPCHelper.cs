﻿using log4net;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Helpers
{
	public static class NPCHelper
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
		/// Modded NPC types that have thrown exceptions during the caching process
		/// </summary>
		private static int[] badModNPCs;

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
				case NPCID.Bee:
				case NPCID.BeeSmall:
				case NPCID.EaterofWorldsBody:
				case NPCID.EaterofWorldsTail:
				case NPCID.Golem:
				case NPCID.GolemHead:
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
				case NPCID.Probe:
				case NPCID.ServantofCthulhu:
				case NPCID.SkeletronHand:
				case NPCID.TheDestroyerBody:
				case NPCID.TheDestroyerTail:
				case NPCID.TheHungry:
				case NPCID.TheHungryII:
					return true;
				case NPCID.BlueSlime:
				case NPCID.SlimeSpiked:
					return NPC.AnyNPCs(NPCID.KingSlime);
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

		public static bool IsSpawnedFromStatue(NPC npc)
		{
			return npc.SpawnedFromStatue;
		}

		public static void Load()
		{
			LoadNPCCache();
		}

		public static void LogBadModNPCs()
		{
			int length = badModNPCs.Length;
			if (length == 0)
			{
				badModNPCs = null;
				return;
			}

			string line = string.Empty;
			ILog logger = RiskOfSlimeRainMod.Instance.Logger;
			logger.Info("NPCs that were skipped for caching: ");
			for (int i = 0; i < length; i++)
			{
				line += Lang.GetNPCName(badModNPCs[i]).Value + ", ";
				if (line.Length > 100)
				{
					logger.Info(line);
					line = string.Empty;
				}
			}
			badModNPCs = null;
			logger.Info("########");
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

		public static bool AnyInvasion()
		{
			return Main.slimeRain || Main.bloodMoon || Main.eclipse || Main.snowMoon || Main.pumpkinMoon || Main.invasionType != 0 || DD2Event.Ongoing;
		}

		/// <summary>
		/// Loads various data related to NPCs
		/// </summary>
		public static void LoadNPCCache()
		{
			List<int> wormList = new List<int>();
			List<int> buffList = new List<int>();
			List<int> badModNPCsList = new List<int>();

			for (int i = 0; i < NPCLoader.NPCCount; i++)
			{
				try
				{
					bool buffImmune = true;
					NPC npc = new NPC();

					// (maybe) tml bug with modded npcs always counting as loaded, thus checking their texture, which doesn't exist yet
					bool prev = Main.NPCLoaded[i];
					if (prev && i >= Main.maxNPCTypes)
					{
						Main.NPCLoaded[i] = false;
					}
					npc.SetDefaults(i);
					if (prev && i >= Main.maxNPCTypes)
					{
						Main.NPCLoaded[i] = true;
					}

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
					badModNPCsList.Add(i);
				}
			}

			isModdedWormBodyOrTail = wormList.ToArray();
			Array.Sort(isModdedWormBodyOrTail);

			isBuffImmune = buffList.ToArray();
			Array.Sort(isBuffImmune);

			badModNPCs = badModNPCsList.ToArray();
		}

		/// <summary>
		/// Copied from npc.Transform but with scaleOverride, and optional ai[] copy
		/// </summary>
		public static void Transform(NPC npc, int newType, float scaleOverride = -1f, bool maxHealth = false, bool syncAI = false)
		{
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				if (npc.townNPC)
				{
					npc.Transform(newType);
					return;
				}

				bool noValue = false;
				if (npc.value == 0f)
				{
					noValue = true;
				}

				int[] buffTypes = new int[NPC.maxBuffs];
				int[] buffTimes = new int[NPC.maxBuffs];
				for (int i = 0; i < NPC.maxBuffs; i++)
				{
					buffTypes[i] = npc.buffType[i];
					buffTimes[i] = npc.buffTime[i];
				}

				float[] ai = new float[NPC.maxAI];
				for (int i = 0; i < NPC.maxAI; i++)
				{
					ai[i] = npc.ai[i];
				}

				int oldType = npc.type;
				int life = npc.life;
				int lifeMax = npc.lifeMax;
				Vector2 velocity = npc.velocity;
				npc.position.Y += npc.height;
				int spriteDir = npc.spriteDirection;
				bool spawnedFromStatue = npc.SpawnedFromStatue;

				npc.SetDefaults(newType, scaleOverride);

				npc.SpawnedFromStatue = spawnedFromStatue;
				npc.spriteDirection = spriteDir;
				npc.TargetClosest(true);
				npc.velocity = velocity;
				npc.position.Y -= npc.height;
				if (noValue)
				{
					npc.value = 0f;
				}
				if (npc.lifeMax == lifeMax || maxHealth)
				{
					npc.life = npc.lifeMax;
				}
				if (newType == 107 || newType == 108)
				{
					npc.homeTileX = (int)(npc.position.X + (npc.width / 2)) / 16;
					npc.homeTileY = (int)(npc.position.Y + npc.height) / 16;
					npc.homeless = true;
				}

				for (int i = 0; i < NPC.maxBuffs; i++)
				{
					npc.buffType[i] = buffTypes[i];
					npc.buffTime[i] = buffTimes[i];
				}

				if (syncAI)
				{
					for (int i = 0; i < NPC.maxAI; i++)
					{
						npc.ai[i] = ai[i];
					}
				}

				if (Main.netMode == NetmodeID.Server)
				{
					npc.netUpdate = true;
					NetMessage.SendData(MessageID.SyncNPC, number: npc.whoAmI);
					NetMessage.SendData(MessageID.SendNPCBuffs, number: npc.whoAmI);
				}
				npc.TransformVisuals(oldType, newType);

				//Commented out because this won't be used for town NPCs
				//if (NPC.TypeToHeadIndex(npc.type) != -1)
				//{
				//	Main.npc[npc.whoAmI].GivenName = NPC.getNewNPCName(npc.type);
				//}
				//npc.npcNameLookup = 0;
				//if (npc.townNPC)
				//{
				//	npc.homeless = true;
				//}
				//npc.altTexture = 0;
			}
		}

		/// <summary>
		/// Checks if an npc is hostile
		/// </summary>
		public static bool IsHostile(NPC npc)
		{
			return !npc.friendly && !npc.immortal && npc.lifeMax > 5 && !npc.dontTakeDamage && npc.chaseable;
		}

		/// <summary>
		/// Heals the NPC, no netmode check required, make sure to call this on all sides
		/// </summary>
		public static void HealMe(this NPC npc, int heal)
		{
			int clampHeal = Math.Min(heal, npc.lifeMax - npc.life);
			npc.HealEffect(heal);
			npc.life += clampHeal;
		}
	}
}
