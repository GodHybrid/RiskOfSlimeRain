using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using RiskOfSlimeRain.Core.ItemSpawning.NPCSpawning;
using RiskOfSlimeRain.Items.Consumable;
using RiskOfSlimeRain.NPCs.Bosses;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Core.ItemSpawning.ModIntegration
{
	public class BossChecklistManager : ModSystem
	{
		private static readonly Version BossChecklistAPIVersion = new Version(2, 0);
		public static Dictionary<string, BossChecklistBossInfo> moddedBossInfoDict;

		public static bool Loaded => moddedBossInfoDict != null;

		public static bool DoBossChecklistIntegration(out Dictionary<string, BossChecklistBossInfo> bossInfos)
		{
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
						displayName = boss.Value.ContainsKey("displayName") ? boss.Value["displayName"] as LocalizedText : null,

						progression = boss.Value.ContainsKey("progression") ? Convert.ToSingle(boss.Value["progression"]) : 0f,
						downed = boss.Value.ContainsKey("downed") ? boss.Value["downed"] as Func<bool> : () => false,

						isBoss = boss.Value.ContainsKey("isBoss") ? Convert.ToBoolean(boss.Value["isBoss"]) : false,
						isMiniboss = boss.Value.ContainsKey("isMiniboss") ? Convert.ToBoolean(boss.Value["isMiniboss"]) : false,
						isEvent = boss.Value.ContainsKey("isEvent") ? Convert.ToBoolean(boss.Value["isEvent"]) : false,

						npcIDs = boss.Value.ContainsKey("npcIDs") ? boss.Value["npcIDs"] as List<int> : new List<int>(),
						spawnInfo = boss.Value.ContainsKey("spawnInfo") ? boss.Value["spawnInfo"] as Func<LocalizedText> : null,
						spawnItems = boss.Value.ContainsKey("spawnItems") ? boss.Value["spawnItems"] as List<int> : new List<int>(),
						treasureBag = boss.Value.ContainsKey("treasureBag") ? Convert.ToInt32(boss.Value["treasureBag"]) : 0,
						dropRateInfo = boss.Value.ContainsKey("dropRateInfo") ? boss.Value["dropRateInfo"] as List<DropRateInfo> : new List<DropRateInfo>(),
						loot = boss.Value.ContainsKey("loot") ? boss.Value["loot"] as List<int> : new List<int>(),
						collectibles = boss.Value.ContainsKey("collectibles") ? boss.Value["collectibles"] as List<int> : new List<int>(),
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

		public void RegisterBosses()
		{
			if (ModLoader.TryGetMod("BossChecklist", out Mod BossChecklist))
			{
				string textureName = Mod.Name + "/Core/ModIntegration/MagmaWorm_Checklist";
				string headName = Mod.Name + "/Core/ModIntegration/MagmaWorm_Checklist_Head";

				List<int> collection = new List<int>()
				{

				};

				int summonItem = ModContent.ItemType<MagmaWormSummon>();

				/*
				"LogBoss",
				submittedMod, // Mod
				internalName, // Internal Name
				Convert.ToSingle(args[3]), // Prog
				args[4] as Func<bool>, // Downed
				InterpretObjectAsListOfInt(args[5]), // NPC IDs
				args[6] as Dictionary<string, object>
				*/
				BossChecklist.Call(
					"LogBoss",
					Mod,
					"MagmaWorm",
					NPCLootManager.Skeletron + 0.1f, //Progress
					(Func<bool>)(() => RORWorld.downedMagmaWorm),
					ModContent.NPCType<MagmaWormHead>(),
					new Dictionary<string, object>()
					{
						["spawnItems"] = summonItem,
						["collectibles"] = collection,
						["overrideHeadTextures"] = headName,
						["customPortrait"] = (SpriteBatch spriteBatch, Rectangle rect, Color color) => DrawSingleImage(spriteBatch, ModContent.Request<Texture2D>(textureName), rect, color),
						["spawnInfo"] = MagmaWorm.SpawnInfoText
						// Other optional arguments as needed...
					}
				);
			}
		}

		private static void DrawSingleImage(SpriteBatch spriteBatch, Asset<Texture2D> asset, Rectangle rect, Color color)
		{
			var texture = asset.Value;
			Vector2 centered = new Vector2(rect.X + (rect.Width / 2) - (texture.Width / 2), rect.Y + (rect.Height / 2) - (texture.Height / 2));
			spriteBatch.Draw(texture, centered, color);
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
