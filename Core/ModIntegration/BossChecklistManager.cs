using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RiskOfSlimeRain.Core.ItemSpawning.NPCSpawning;
using RiskOfSlimeRain.Items;
using RiskOfSlimeRain.Items.Consumable.Boss;
using RiskOfSlimeRain.NPCs.Bosses;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Core.ItemSpawning.ModIntegration
{
	public static class BossChecklistManager
	{
		private static readonly Version BossChecklistAPIVersion = new Version(1, 1);
		public static Dictionary<string, BossChecklistBossInfo> moddedBossInfoDict;

		public static bool Loaded => moddedBossInfoDict != null;

		public static bool DoBossChecklistIntegration(out Dictionary<string, BossChecklistBossInfo> bossInfos)
		{
			//Make sure to call this method in PostAddRecipes or later for best results
			Mod BossChecklist = ModLoader.GetMod("BossChecklist");
			if (BossChecklist != null && BossChecklist.Version >= BossChecklistAPIVersion)
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

		//Credit to jopojelly
		/// <summary>
		/// Makes alpha on .png textures actually properly rendered
		/// </summary>
		public static void PremultiplyTexture(Texture2D texture)
		{
			Color[] buffer = new Color[texture.Width * texture.Height];
			texture.GetData(buffer);
			for (int i = 0; i < buffer.Length; i++)
			{
				buffer[i] = Color.FromNonPremultiplied(buffer[i].R, buffer[i].G, buffer[i].B, buffer[i].A);
			}
			texture.SetData(buffer);
		}

		public static void RegisterBosses()
		{
			Mod BossChecklist = ModLoader.GetMod("BossChecklist");
			if (BossChecklist != null && BossChecklist.Version >= new Version(1, 0))
			{
				Mod mod = RiskOfSlimeRainMod.Instance;
				string textureName = mod.Name + "/Core/ModIntegration/MagmaWorm_Checklist";
				BossChecklist.Call("AddBoss", //Command
					NPCLootManager.Skeletron + 0.1f, //Progress
					ModContent.NPCType<MagmaWormHead>(),
					mod,
					"Magma Worm",
					(Func<bool>)(() => RORWorld.downedMagmaWorm),
					ModContent.ItemType<WarbannerRemover>(),
					new List<int>(),
					ModContent.ItemType<BurningWitness>(), //Loot
					"summon with this",
					null,
					textureName, null, null);

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

		public static void Unload()
		{
			moddedBossInfoDict = null;
		}
	}
}
