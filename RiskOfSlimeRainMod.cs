using RiskOfSlimeRain.Core.NPCEffects;
using RiskOfSlimeRain.Core.ROREffects;
using RiskOfSlimeRain.Core.Warbanners;
using RiskOfSlimeRain.Effects;
using RiskOfSlimeRain.Network;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using WebmilioCommons.Networking;

namespace RiskOfSlimeRain
{
	public class RiskOfSlimeRainMod : Mod
	{
		public static RiskOfSlimeRainMod Instance => ModContent.GetInstance<RiskOfSlimeRainMod>();

		public RiskOfSlimeRainMod()
		{
			//git is gay
		}

		public override void Load()
		{
			ROREffectManager.Load();
			NPCEffectManager.Load();
			ShaderManager.Load();
			RORInterfaceLayers.Load();
		}

		public override void PostSetupContent()
		{
			RORNetworkTypeSerializers.Load();
		}

		public class BossChecklistBossInfo
		{
			internal string key = ""; // equal to "modSource internalName"
			internal string modSource = "";
			internal string internalName = "";
			internal string displayName = "";
			internal float progression = 0f; // See https://github.com/JavidPack/BossChecklist/blob/master/BossTracker.cs#L13 for vanilla boss values
			internal Func<bool> downed = () => false;
			internal bool isBoss = false;
			internal bool isMiniboss = false;
			internal bool isEvent = false;
			internal List<int> npcIDs = new List<int>(); // Does not include minions, only npcids that count towards the NPC still being alive.
			internal List<int> spawnItem = new List<int>();
			internal List<int> loot = new List<int>();
			internal List<int> collection = new List<int>();

			public override string ToString()
			{
				return progression + key;
			}
		}

		public static bool DoBossChecklistIntegration()
		{
			// Make sure to call this method in PostAddRecipes or later for best results
			Mod BossChecklist = ModLoader.GetMod("BossChecklist");
			if (BossChecklist != null && BossChecklist.Version >= BossChecklistAPIVersion)
			{
				object currentBossInfoResponse = BossChecklist.Call("GetBossInfoDictionary", BossChecklistAPIVersion.ToString());
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
			return false;
		}

		private static readonly Version BossChecklistAPIVersion = new Version(1, 1);
		public static Dictionary<string, BossChecklistBossInfo> bossInfos;
		public static Dictionary<string, BossChecklistBossInfo> relevantBossInfoDict;
		public static bool bcIntegration = false;

		public override void PostAddRecipes()
		{
			if (DoBossChecklistIntegration())
			{
				bcIntegration = true;

				var bossesEnumerable = bossInfos.Where(boss => boss.Value.isBoss);
				relevantBossInfoDict = bossesEnumerable.ToDictionary(boss => boss.Key, boss => boss.Value);

				List<int> unwantedBosses = new List<int>() { WorldGen.crimson ? NPCID.EaterofWorldsHead : NPCID.BrainofCthulhu, NPCID.DD2Betsy };

				var otherBossesEnumerable = relevantBossInfoDict.Where(boss => boss.Value.isBoss && boss.Value.npcIDs.Any(id => unwantedBosses.Contains(id)));
				var otherBossesDict = otherBossesEnumerable.ToDictionary(boss => boss.Key, boss => boss.Value);

				foreach (var boss in otherBossesDict)
				{
					relevantBossInfoDict.Remove(boss.Key);
				}
			}
			else
			{
				bossInfos = new Dictionary<string, BossChecklistBossInfo>();
			}
		}

		public static void BossesDefeated(out int preHM, out int HM)
		{
			var enumerator = relevantBossInfoDict.Where(boss => boss.Value.downed());
			var downed = enumerator.ToDictionary(boss => boss.Key, boss => boss.Value);
			preHM = downed.Count(boss => boss.Value.progression < 6f);
			HM = downed.Count(boss => boss.Value.progression >= 6f);
		}

		public override void Unload()
		{
			bossInfos = null;

			OtherUnload();
		}

		public static void OtherUnload()
		{
			ROREffectManager.Unload();
			NPCEffectManager.Unload();
			ShaderManager.Unload();
			RORInterfaceLayers.Unload();
			WarbannerManager.Unload();
		}

		public override void AddRecipeGroups()
		{
			RecipeGroup HMTier3Bar_Group = new RecipeGroup(() => Language.GetTextValue("LegacyMisc.37") + " Tier 3 Hardmode bar", new int[]
			{
				ItemID.AdamantiteBar,
				ItemID.TitaniumBar
			});
			RecipeGroup.RegisterGroup("RoR:Tier3HMBar", HMTier3Bar_Group);

			RecipeGroup SilvTung_Group = new RecipeGroup(() => Language.GetTextValue("LegacyMisc.37") + " Tier 3 incipient bar", new int[]
				{
					ItemID.SilverBar,
					ItemID.TungstenBar
				});
			RecipeGroup.RegisterGroup("RoR:SilvTungBar", SilvTung_Group);

			RecipeGroup GoldPlat_Group = new RecipeGroup(() => Language.GetTextValue("LegacyMisc.37") + " Tier 4 incipient bar", new int[]
			{
				ItemID.PlatinumBar,
				ItemID.GoldBar
			});
			RecipeGroup.RegisterGroup("RoR:GoldPlatBar", GoldPlat_Group);

			RecipeGroup Chest_Group = new RecipeGroup(() => Language.GetTextValue("LegacyMisc.37") + " Chest", new int[]
			{
				#region Chest_List
				ItemID.BlueDungeonChest,
				ItemID.BoneChest,
				ItemID.BorealWoodChest,
				ItemID.CactusChest,
				ItemID.Chest,
				ItemID.CorruptionChest,
				ItemID.CrimsonChest,
				ItemID.CrystalChest,
				ItemID.DynastyChest,
				ItemID.EbonwoodChest,
				ItemID.FleshChest,
				ItemID.FrozenChest,
				ItemID.GlassChest,
				ItemID.GoldChest,
				ItemID.GoldenChest,
				ItemID.GraniteChest,
				ItemID.GreenDungeonChest,
				ItemID.HallowedChest,
				ItemID.HoneyChest,
				ItemID.IceChest,
				ItemID.IvyChest,
				ItemID.JungleChest,
				ItemID.LihzahrdChest,
				ItemID.LivingWoodChest,
				ItemID.MarbleChest,
				ItemID.MartianChest,
				ItemID.MeteoriteChest,
				ItemID.MushroomChest,
				ItemID.ObsidianChest,
				ItemID.PalmWoodChest,
				ItemID.PearlwoodChest,
				ItemID.PinkDungeonChest,
				ItemID.PumpkinChest,
				ItemID.RichMahoganyChest,
				ItemID.ShadewoodChest,
				ItemID.ShadowChest,
				ItemID.SkywareChest,
				ItemID.SlimeChest,
				ItemID.SpookyChest,
				ItemID.SteampunkChest,
				ItemID.WaterChest,
				ItemID.WebCoveredChest
				#endregion
			});
			RecipeGroup.RegisterGroup("RoR:AnyChest", Chest_Group);

			RecipeGroup Jellyfish_Group = new RecipeGroup(() => Language.GetTextValue("LegacyMisc.37") + " Jellyfish", new int[]
			{
				ItemID.BlueJellyfish,
				ItemID.GreenJellyfish,
				ItemID.PinkJellyfish
			});
			RecipeGroup.RegisterGroup("RoR:Jellyfish", Jellyfish_Group);

			RecipeGroup EvilWater_Group = new RecipeGroup(() => Language.GetTextValue("LegacyMisc.37") + " Evil water", new int[]
			{
				ItemID.UnholyWater,
				ItemID.BloodWater
			});
			RecipeGroup.RegisterGroup("RoR:EvilWater", EvilWater_Group);

			RecipeGroup EvilMat_Group = new RecipeGroup(() => Language.GetTextValue("LegacyMisc.37") + " common evil material", new int[]
			{
				ItemID.Vertebrae,
				ItemID.RottenChunk
			});
			RecipeGroup.RegisterGroup("RoR:EvilMaterial", EvilMat_Group);

			RecipeGroup FastBoots_Group = new RecipeGroup(() => Language.GetTextValue("LegacyMisc.37") + " common speed-up boots", new int[]
			{
				ItemID.FlurryBoots,
				ItemID.HermesBoots,
				ItemID.SailfishBoots
			});
			RecipeGroup.RegisterGroup("RoR:FastBoots", FastBoots_Group);

			RecipeGroup EvilShroom_Group = new RecipeGroup(() => Language.GetTextValue("LegacyMisc.37") + " evil mushroom", new int[]
			{
				ItemID.ViciousMushroom,
				ItemID.VileMushroom
			});
			RecipeGroup.RegisterGroup("RoR:EvilMushrooms", EvilShroom_Group);
		}

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			RORInterfaceLayers.ModifyInterfaceLayers(layers);
		}

		public override void HandlePacket(BinaryReader reader, int whoAmI)
		{
			NetworkPacketLoader.Instance.HandlePacket(reader, whoAmI);
		}
	}
}
