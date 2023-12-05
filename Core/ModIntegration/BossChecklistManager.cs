using RiskOfSlimeRain.Core.ItemSpawning.NPCSpawning;
using RiskOfSlimeRain.Items.Consumable;
using RiskOfSlimeRain.Items.Consumable.Boss;
using RiskOfSlimeRain.NPCs.Bosses;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Core.ItemSpawning.ModIntegration
{
	public class BossChecklistManager : ModSystem
	{
		private static readonly Version BossChecklistAPIVersion = new Version(1, 1);
		public static Dictionary<string, BossChecklistBossInfo> moddedBossInfoDict;

		public static bool Loaded => moddedBossInfoDict != null;

		public static bool DoBossChecklistIntegration(out Dictionary<string, BossChecklistBossInfo> bossInfos)
		{
			//TODO 1.4.4
			//Make sure to call this method in PostAddRecipes or later for best results
			if (ModLoader.TryGetMod("BossChecklist", out Mod BossChecklist) && BossChecklist.Version >= BossChecklistAPIVersion)
			{
				object currentBossInfoResponse = BossChecklist.Call("GetBossInfoDictionary", RiskOfSlimeRainMod.Instance, BossChecklistAPIVersion.ToString());
				if (currentBossInfoResponse is Dictionary<string, Dictionary<string, object>> bossInfoList)
				{
					bossInfos = bossInfoList.ToDictionary(boss => boss.Key, boss => new BossChecklistBossInfo()
					{
						key = boss.Value.ContainsKey("key") ? boss.Value["key"] as string : "",
						modSource = boss.Value.ContainsKey("modSource") ? boss.Value["modSource"] as string : "",
						internalName = boss.Value.ContainsKey("internalName") ? boss.Value["internalName"] as string : "",
						displayName = boss.Value.ContainsKey("displayName") ? boss.Value["displayName"] as string : "",
						progression = boss.Value.ContainsKey("progression") ? Convert.ToSingle(boss.Value["progression"]) : 0f,
						downed = boss.Value.ContainsKey("downed") ? boss.Value["downed"] as Func<bool> : () => false,
						isBoss = boss.Value.ContainsKey("isBoss") ? Convert.ToBoolean(boss.Value["isBoss"]) : false,
						isMiniboss = boss.Value.ContainsKey("isMiniboss") ? Convert.ToBoolean(boss.Value["isMiniboss"]) : false,
						isEvent = boss.Value.ContainsKey("isEvent") ? Convert.ToBoolean(boss.Value["isEvent"]) : false,
						npcIDs = boss.Value.ContainsKey("npcIDs") ? boss.Value["npcIDs"] as List<int> : new List<int>(),
						spawnItem = boss.Value.ContainsKey("spawnItem") ? boss.Value["spawnItem"] as List<int> : new List<int>(),
						loot = boss.Value.ContainsKey("loot") ? boss.Value["loot"] as List<int> : new List<int>(),
						collection = boss.Value.ContainsKey("collection") ? boss.Value["collection"] as List<int> : new List<int>(),
					});
					return true;
				}
			}
			bossInfos = null;
			return false;
		}

		/// <summary>
		/// Returns true if the NPC is registered as a boss in the KeyValuePair
		/// </summary>
		public static bool Exists(NPC npc, KeyValuePair<string, BossChecklistBossInfo> boss)
		{
			return boss.Value.npcIDs.Contains(npc.type);
		}

		/// <summary>
		/// Returns the info of the NPC if it exists in Boss Checklist, null otherwise
		/// </summary>
		public static BossChecklistBossInfo GetBossInfoOfNPC(NPC npc)
		{
			return moddedBossInfoDict.FirstOrDefault(boss => Exists(npc, boss)).Value;
		}

		public static void RegisterBosses()
		{
			//TODO 1.4.4
			if (ModLoader.TryGetMod("BossChecklist", out Mod BossChecklist) && BossChecklist.Version >= new Version(1, 0))
			{
				Mod mod = RiskOfSlimeRainMod.Instance;
				string tooltip = MagmaWormSummon.tooltip;
				tooltip += ". Drops 'Burning Witness' on first death, then with 1% chance";
				string textureName = mod.Name + "/Core/ModIntegration/MagmaWorm_Checklist";
				string headName = mod.Name + "/Core/ModIntegration/MagmaWorm_Checklist_Head";
				BossChecklist.Call("AddBoss", //Command
					NPCLootManager.Skeletron + 0.1f, //Progress
					ModContent.NPCType<MagmaWormHead>(),
					mod,
					"Magma Worm",
					(Func<bool>)(() => RORWorld.downedMagmaWorm),
					ModContent.ItemType<MagmaWormSummon>(),
					new List<int>(),
					ModContent.ItemType<BurningWitness>(), //Loot
					tooltip,
					null,
					textureName,
					headName,
					null);

			}
		}

		public static void LoadBossInfo()
		{
			if (DoBossChecklistIntegration(out Dictionary<string, BossChecklistBossInfo> bossInfos))
			{
				var moddedBossesEnumerable = bossInfos.Where(boss => boss.Value.modSource != "Terraria" && boss.Value.modSource != RiskOfSlimeRainMod.Instance.Name && boss.Value.isBoss);
				moddedBossInfoDict = moddedBossesEnumerable.ToDictionary(boss => boss.Key, boss => boss.Value);
			}
		}

		public override void PostSetupContent()
		{
			RegisterBosses();
		}

		public override void PostAddRecipes()
		{
			LoadBossInfo();
		}

		public override void OnModUnload()
		{
			moddedBossInfoDict = null;
		}
	}
}
