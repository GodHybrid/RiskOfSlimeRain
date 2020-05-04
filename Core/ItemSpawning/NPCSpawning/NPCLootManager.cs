﻿using RiskOfSlimeRain.Core.ROREffects;
using RiskOfSlimeRain.Network;
using RiskOfSlimeRain.NPCs;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader.IO;

namespace RiskOfSlimeRain.Core.ItemSpawning.NPCSpawning
{
	public static class NPCLootManager
	{
		//Contains the "progression" values of each relevant downed vanilla boss
		public static List<float> vanillaDowned;

		//Contains the "key"  of each downed modded boss
		public static Dictionary<string, float> moddedDowned;

		public const int RepeatedDropRate = 100;
		public const float HMDropRateMultiplierForPreHMBosses = 2;

		//Unique float identifiers for each boss, copy from BossChecklist
		public const float KingSlime = 1f;
		public const float EyeOfCthulhu = 2f;
		public const float EvilBoss = 3f;
		public const float QueenBee = 4f;
		public const float Skeletron = 5f;
		public const float WallOfFlesh = 6f;

		public const float TheTwins = 7f;
		public const float TheDestroyer = 8f;
		public const float SkeletronPrime = 9f;
		public const float Plantera = 10f;
		public const float Golem = 11f;
		public const float DukeFishron = 12f;
		public const float LunaticCultist = 13f;
		public const float Moonlord = 14f;

		public const int BossCountPreHM = 6;
		public const int BossCountHM = 8;

		public static void DropItem(NPC npc)
		{
			if (!npc.boss) return;

			bool isPreHM = IsPreHardmode(npc);
			if (!CheckProgression(isPreHM)) return;
			bool firstKill = false;
			bool isBoss = npc.modNPC == null ? CheckVanilla(npc, ref firstKill) : CheckModded(npc, ref firstKill);

			if (!isBoss) return;
			if (firstKill)
			{
				//First time defeated
				DropOneItemPerPlayer(npc);
			}
			else
			{
				//Already defeated
				int random = RepeatedDropRate;
				if (Main.hardMode && isPreHM)
				{
					random = (int)(random * HMDropRateMultiplierForPreHMBosses);
				}

				if (Main.rand.NextBool(random))
				{
					DropOneItemPerPlayer(npc);
				}
			}
		}

		public static void DropOneItemPerPlayer(NPC npc)
		{
			RORRarity rarity = RORRarity.Common;
			//float rarityRand = Main.rand.NextFloat();
			//if (rarityRand < 0.05f)
			//{
			//	rarity = ROREffectRarity.Rare;
			//}
			//else if (rarityRand < 0.25f)
			//{
			//	rarity = ROREffectRarity.Uncommon;
			//}
			//else common

			List<int> items = ROREffectManager.GetItemTypesOfRarity(rarity);
			if (items.Count <= 0) return; //Item list empty, no items to drop! (mod is not complete yet)

			int itemTypeFunc() => Main.rand.Next(items);
			RORGlobalNPC.DropItemInstanced(npc, npc.position, npc.Hitbox.Size(), itemTypeFunc);
		}

		//if (preHM) return f <= WallOfFlesh; else return f > WallOfFlesh;
		private static bool ProgressIsPostWallOfFlesh(bool preHM, float f) => preHM ^ f > WallOfFlesh;

		public static bool CheckProgression(bool preHM)
		{
			if (!BossChecklistManager.Loaded) return true;
			int vanilla = vanillaDowned.Count(f => ProgressIsPostWallOfFlesh(preHM, f));

			//moddedDowned contains things even if mods are disabled, preserving behavior
			int modded = moddedDowned.Count;

			int compare = preHM ? BossCountPreHM : BossCountHM;
			//No point counting bosses if the total modded boss count is less
			if (modded >= compare)
			{
				//Count how many beaten bosses are pre/HM
				modded = moddedDowned.Count(boss => ProgressIsPostWallOfFlesh(preHM, boss.Value));
				if (modded + vanilla >= compare * 2)
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Returns true if this NPC is one of the relevant bosses, also sets firstKill to true if never downed before
		/// </summary>
		private static bool CheckVanilla(NPC npc, ref bool firstKill)
		{
			bool drop = false;
			switch (npc.type)
			{
				case NPCID.KingSlime:
					if (vanillaDowned.BinarySearch(KingSlime) < 0)
					{
						vanillaDowned.Add(KingSlime);
						firstKill = true;
					}
					drop = true;
					break;
				case NPCID.EyeofCthulhu:
					if (vanillaDowned.BinarySearch(EyeOfCthulhu) < 0)
					{
						vanillaDowned.Add(EyeOfCthulhu);
						firstKill = true;
					}
					drop = true;
					break;
				case NPCID.BrainofCthulhu:
				case NPCID.EaterofWorldsBody:
				case NPCID.EaterofWorldsHead:
				case NPCID.EaterofWorldsTail:
					if (WorldGen.crimson)
					{
						if (vanillaDowned.BinarySearch(EvilBoss) < 0)
						{
							vanillaDowned.Add(EvilBoss);
							firstKill = true;
						}
						drop = true;
					}
					else
					{
						if (npc.boss)
						{
							if (vanillaDowned.BinarySearch(EvilBoss) < 0)
							{
								vanillaDowned.Add(EvilBoss);
								firstKill = true;
							}
							drop = true;
						}
					}
					break;
				case NPCID.QueenBee:
					if (vanillaDowned.BinarySearch(QueenBee) < 0)
					{
						vanillaDowned.Add(QueenBee);
						firstKill = true;
					}
					drop = true;
					break;
				case NPCID.SkeletronHead:
					if (vanillaDowned.BinarySearch(Skeletron) < 0)
					{
						vanillaDowned.Add(Skeletron);
						firstKill = true;
					}
					drop = true;
					break;
				case NPCID.WallofFlesh:
					if (vanillaDowned.BinarySearch(WallOfFlesh) < 0)
					{
						vanillaDowned.Add(WallOfFlesh);
						firstKill = true;
					}
					drop = true;
					break;
				case NPCID.TheDestroyer:
					if (vanillaDowned.BinarySearch(TheDestroyer) < 0)
					{
						vanillaDowned.Add(TheDestroyer);
						firstKill = true;
					}
					drop = true;
					break;
				case NPCID.SkeletronPrime:
					if (vanillaDowned.BinarySearch(SkeletronPrime) < 0)
					{
						vanillaDowned.Add(SkeletronPrime);
						firstKill = true;
					}
					drop = true;
					break;
				case NPCID.Retinazer:
				case NPCID.Spazmatism:
					if (npc.type == NPCID.Retinazer && !NPC.AnyNPCs(NPCID.Spazmatism) || npc.type == NPCID.Spazmatism && !NPC.AnyNPCs(NPCID.Retinazer))
					{
						if (vanillaDowned.BinarySearch(TheTwins) < 0)
						{
							vanillaDowned.Add(TheTwins);
							firstKill = true;
						}
						drop = true;
					}
					break;
				case NPCID.Plantera:
					if (vanillaDowned.BinarySearch(Plantera) < 0)
					{
						vanillaDowned.Add(Plantera);
						firstKill = true;
					}
					drop = true;
					break;
				case NPCID.Golem:
					if (vanillaDowned.BinarySearch(Golem) < 0)
					{
						vanillaDowned.Add(Golem);
						firstKill = true;
					}
					drop = true;
					break;
				case NPCID.DukeFishron:
					if (vanillaDowned.BinarySearch(DukeFishron) < 0)
					{
						vanillaDowned.Add(DukeFishron);
						firstKill = true;
					}
					drop = true;
					break;
				case NPCID.CultistBoss:
					if (vanillaDowned.BinarySearch(LunaticCultist) < 0)
					{
						vanillaDowned.Add(LunaticCultist);
						firstKill = true;
					}
					drop = true;
					break;
				case NPCID.MoonLordCore:
					if (vanillaDowned.BinarySearch(Moonlord) < 0)
					{
						vanillaDowned.Add(Moonlord);
						firstKill = true;
					}
					drop = true;
					break;
				default:
					break;
			}
			if (firstKill)
			{
				vanillaDowned.Sort();
				new UpdateDownedPacket(true).Send();
			}
			return drop;
		}

		/// <summary>
		/// Returns true if this NPC is one of the registered modded bosses, also sets firstKill to true if never downed before
		/// </summary>
		public static bool CheckModded(NPC npc, ref bool firstKill)
		{
			bool drop = false;
			if (!BossChecklistManager.Loaded) return false;

			var boss = BossChecklistManager.GetBossInfoOfNPC(npc);
			if (boss != null)
			{
				if (!moddedDowned.ContainsKey(boss.key))
				{
					moddedDowned.Add(boss.key, boss.progression);
					new UpdateDownedPacket(false).Send();
					firstKill = true;
				}
				drop = true;
			}
			return drop;
		}

		/// <summary>
		/// Returns true if the NPC belongs in pre-hardmode
		/// </summary>
		public static bool IsPreHardmode(NPC npc)
		{
			if (npc.modNPC == null)
			{
				switch (npc.type)
				{
					case NPCID.KingSlime:
					case NPCID.EyeofCthulhu:
					case NPCID.BrainofCthulhu:
					case NPCID.EaterofWorldsBody:
					case NPCID.EaterofWorldsHead:
					case NPCID.EaterofWorldsTail:
					case NPCID.QueenBee:
					case NPCID.SkeletronHead:
					case NPCID.WallofFlesh:
						return true;
				}
			}
			else
			{
				if (BossChecklistManager.Loaded)
				{
					return BossChecklistManager.moddedBossInfoDict.Any(boss => BossChecklistManager.Exists(npc, boss) && boss.Value.progression < WallOfFlesh);
				}
			}
			return false;
		}

		public static string GetDisplayNameOfEarliestNonBeatenBoss(out float progression)
		{
			string vanillaName = GetNameOfEarliestNonBeatenVanillaBoss(out float vanillaProgression);
			if (BossChecklistManager.Loaded)
			{
				string moddedName = GetNameOfEarliestNonBeatenModdedBoss(out float moddedProgression);
				if (moddedProgression < vanillaProgression)
				{
					progression = moddedProgression;
					return GetTextFromDisplayName(moddedName);
				}
			}
			progression = vanillaProgression;
			return GetTextFromDisplayName(vanillaName);
		}

		public static string GetNameOfEarliestNonBeatenModdedBoss(out float progression)
		{
			string ret = "You have beaten all modded bosses!";
			progression = float.MaxValue;
			if (BossChecklistManager.moddedBossInfoDict.Count <= 0)
			{
				return "There are no modded bosses to beat!";
			}
			var bossInfo = BossChecklistManager.moddedBossInfoDict.FirstOrDefault(boss => !moddedDowned.ContainsKey(boss.Key)).Value; //Ordered by progression
			if (bossInfo != null)
			{
				ret = bossInfo.displayName;
				progression = bossInfo.progression;
			}
			return ret;
		}

		public static string GetNameOfEarliestNonBeatenVanillaBoss(out float progression)
		{
			string ret = "You have beaten all bosses!";
			progression = float.MaxValue;
			if (vanillaDowned.BinarySearch(KingSlime) < 0)
			{
				progression = KingSlime;
				ret = "$NPCName.KingSlime";
			}
			else if (vanillaDowned.BinarySearch(EyeOfCthulhu) < 0)
			{
				progression = EyeOfCthulhu;
				ret = "$NPCName.EyeofCthulhu";
			}
			else if (vanillaDowned.BinarySearch(EvilBoss) < 0)
			{
				progression = EvilBoss;
				ret = WorldGen.crimson ? "NPCName.BrainofCthulhu" : "$NPCName.EaterofWorldsHead";
			}
			else if (vanillaDowned.BinarySearch(QueenBee) < 0)
			{
				progression = QueenBee;
				ret = "$NPCName.QueenBee";
			}
			else if (vanillaDowned.BinarySearch(Skeletron) < 0)
			{
				progression = Skeletron;
				ret = "$NPCName.SkeletronHead";
			}
			else if (vanillaDowned.BinarySearch(WallOfFlesh) < 0)
			{
				progression = WallOfFlesh;
				ret = "$NPCName.WallofFlesh";
			}
			//Hardmode
			else if (vanillaDowned.BinarySearch(TheTwins) < 0)
			{
				progression = TheTwins;
				ret = "$NPCName.TheTwins";
			}
			else if (vanillaDowned.BinarySearch(TheDestroyer) < 0)
			{
				progression = TheDestroyer;
				ret = "$NPCName.TheDestroyer";
			}
			else if (vanillaDowned.BinarySearch(SkeletronPrime) < 0)
			{
				progression = SkeletronPrime;
				ret = "$NPCName.SkeletronPrime";
			}
			else if (vanillaDowned.BinarySearch(Plantera) < 0)
			{
				progression = Plantera;
				ret = "$NPCName.Plantera";
			}
			else if (vanillaDowned.BinarySearch(Golem) < 0)
			{
				progression = Golem;
				ret = "$NPCName.Golem";
			}
			else if (vanillaDowned.BinarySearch(DukeFishron) < 0)
			{
				progression = DukeFishron;
				ret = "$NPCName.DukeFishron";
			}
			else if (vanillaDowned.BinarySearch(LunaticCultist) < 0)
			{
				progression = LunaticCultist;
				ret = "$NPCName.CultistBoss";
			}
			else if (vanillaDowned.BinarySearch(Moonlord) < 0)
			{
				progression = Moonlord;
				ret = "$NPCName.MoonLord";
			}
			return ret;
		}

		public static string GetTextFromDisplayName(string displayName)
		{
			return displayName?.StartsWith("$") == true ? Language.GetTextValue(displayName.Substring(1)) : displayName;
		}

		public static void Init()
		{
			vanillaDowned = new List<float>();
			moddedDowned = new Dictionary<string, float>();
		}

		public static void Load(TagCompound tag)
		{
			var vlist = tag.GetList<float>("vanillaDowned");
			vanillaDowned = (List<float>)vlist;
			//for (int i = 0; i < Moonlord; i++)
			//{
			//	vanillaDowned.Add(i + 1);
			//}
			vanillaDowned.Sort();

			var moddedDownedCompounds = tag.GetList<TagCompound>("moddedDownedCompounds");
			moddedDowned = new Dictionary<string, float>();
			foreach (var t in moddedDownedCompounds)
			{
				string bosskey = t.GetString("bosskey");
				float progression = t.GetFloat("progression");
				//Update progression if it changed
				if (BossChecklistManager.Loaded && BossChecklistManager.moddedBossInfoDict.TryGetValue(bosskey, out BossChecklistBossInfo value))
				{
					progression = value.progression;
				}
				moddedDowned.Add(bosskey, progression);
			}

			//var mList = tag.GetList<string>("moddedDowned");
			//moddedDowned = (List<string>)mList;
			//moddedDowned.Clear();
			//moddedDowned.Sort();
		}

		public static void Save(TagCompound tag)
		{
			tag.Add("vanillaDowned", vanillaDowned);

			List<TagCompound> moddedDownedCompounds = new List<TagCompound>();
			foreach (var entry in moddedDowned)
			{
				moddedDownedCompounds.Add(new TagCompound()
				{
					{"bosskey", entry.Key },
					{"progression", entry.Value },
				});
			}
			tag.Add("moddedDownedCompounds", moddedDownedCompounds);
		}

		/// <summary>
		/// Specify true to send the vanillaDowned, false for moddedDowned
		/// </summary>
		public static void NetSend(BinaryWriter writer, bool vanilla)
		{
			if (vanilla)
			{
				writer.Write(vanillaDowned.Count);
				for (int i = 0; i < vanillaDowned.Count; i++)
				{
					writer.Write((float)vanillaDowned[i]);
				}
			}
			else
			{
				writer.Write(moddedDowned.Count);
				foreach (var entry in moddedDowned)
				{
					writer.Write(entry.Key);
					writer.Write(entry.Value);
				}
			}
		}

		/// <summary>
		/// Specify true to receive the vanillaDowned, false for moddedDowned
		/// </summary>
		public static void NetReceive(BinaryReader reader, bool vanilla)
		{
			if (vanilla)
			{
				vanillaDowned = new List<float>();
				int count = reader.ReadInt32();
				for (int i = 0; i < count; i++)
				{
					float value = reader.ReadSingle();
					vanillaDowned.Add(value);
				}
			}
			else
			{
				moddedDowned = new Dictionary<string, float>();
				int count = reader.ReadInt32();
				for (int i = 0; i < count; i++)
				{
					string key = reader.ReadString();
					float progression = reader.ReadSingle();
					moddedDowned.Add(key, progression);
				}
			}
		}

		public static void Unload()
		{
			vanillaDowned = null;
			moddedDowned = null;
		}
	}
}
