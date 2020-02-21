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

		internal const BindingFlags BF_STATIC = BindingFlags.Static | BindingFlags.NonPublic;
		internal const BindingFlags BF_INSTANCE = BindingFlags.Instance | BindingFlags.NonPublic;

		//static List<BossInfo>

		public static IList bossList;

		public static List<Func<bool>> downedPreHMBossList;

		public static List<Func<bool>> downedHMBossList;

		public static void BossesDefeated(out int preHM, out int HM)
		{
			preHM = 0;
			HM = 0;
			foreach (var downed in downedPreHMBossList)
			{
				if (downed()) preHM++;
			}
			if (NPC.downedBoss2) preHM--;

			foreach (var downed in downedHMBossList)
			{
				if (downed()) HM++;
			}
		}

		public override void PostSetupContent()
		{
			RORNetworkTypeSerializers.Load();

			Type BossChecklistModType = ModLoader.GetMod("BossChecklist").GetType();

			FieldInfo bossTrackerField = BossChecklistModType.GetField("bossTracker", BF_STATIC);
			Type bossTrackerType = bossTrackerField.FieldType;
			FieldInfo SortedBossesField = bossTrackerType.GetField("SortedBosses", BF_INSTANCE);

			Type BossInfoType = ModLoader.GetMod("BossChecklist").Code.GetType("BossChecklist.BossInfo");

			//Type t;
			//Type listType = typeof(List<>);
			//Type constructedListType = listType.MakeGenericType(bossTrackerType);


			//var instancedList = (IList)typeof(List<>)//Create a Generic List that can hold our type
			// .MakeGenericType(SortedBossesType)
			// .GetConstructor(Type.EmptyTypes)
			// .Invoke(null);

			object bossTracker = bossTrackerField.GetValue(null);

			IList bossInfoList = (IList)SortedBossesField.GetValue(bossTracker);
			bossList = (IList)typeof(List<>)//Create a Generic List that can hold our type
				.MakeGenericType(BossInfoType)
				.GetConstructor(Type.EmptyTypes)
				.Invoke(null);

			downedPreHMBossList = new List<Func<bool>>();

			downedHMBossList = new List<Func<bool>>();

			foreach (object bossInfo in bossInfoList)
			{
				FieldInfo typeField = BossInfoType.GetField("type", BF_INSTANCE);
				Type EntryTypeType = typeField.FieldType;
				object type = typeField.GetValue(bossInfo);

				object boss = Enum.Parse(EntryTypeType, "Boss");

				if (type.Equals(boss))
				{
					bossList.Add(bossInfo);

					FieldInfo progressionField = BossInfoType.GetField("progression", BF_INSTANCE);
					float progression = (float)progressionField.GetValue(bossInfo);

					FieldInfo downedField = BossInfoType.GetField("downed", BF_INSTANCE);
					Func<bool> downed = (Func<bool>)downedField.GetValue(bossInfo);

					if (progression <= 6f)
					{
						downedPreHMBossList.Add(downed);
					}
					else
					{
						downedHMBossList.Add(downed);
					}
				}
			}
		}

		internal class BossInfo
		{
			internal string key = "";
			internal float progression = 0f;
			internal string displayName = "";
			internal List<int> npcIDs = new List<int>();
			internal Func<bool> downed = () => false;

			internal bool isBoss = false;
			internal bool isMiniboss = false;
			internal bool isEvent = false;

			internal List<int> spawnItem = new List<int>();
			internal List<int> loot = new List<int>();
			internal List<int> collection = new List<int>();

			internal BossInfo()
			{

			}
		}

		List<BossInfo> bossInfos = new List<BossInfo>();

		public override void PostAddRecipes()
		{
			Mod bc = ModLoader.GetMod("BossChecklist");
			if (bc != null)
			{
				object data = bc.Call("GetCurrentBossInfo");

				if (data is List<Dictionary<string, object>> list)
				{
					foreach (var boss in list)
					{
						var tinyBossInfo = new BossInfo()
						{
							key = boss.ContainsKey("key") ? boss["key"] as string : "",
							progression = boss.ContainsKey("progression") ? Convert.ToSingle(boss["progression"]) : 0f,
							displayName = boss.ContainsKey("displayName") ? boss["displayName"] as string : "",
							npcIDs = boss.ContainsKey("npcIDs") ? boss["npcIDs"] as List<int> : new List<int>(),
							downed = boss.ContainsKey("downed") ? boss["downed"] as Func<bool> : () => false,

							isBoss = boss.ContainsKey("isBoss") ? Convert.ToBoolean(boss["isBoss"]) : false,
							isMiniboss = boss.ContainsKey("isMiniboss") ? Convert.ToBoolean(boss["isMiniboss"]) : false,
							isEvent = boss.ContainsKey("isEvent") ? Convert.ToBoolean(boss["isEvent"]) : false,

							spawnItem = boss.ContainsKey("spawnItem") ? boss["spawnItem"] as List<int> : new List<int>(),
							loot = boss.ContainsKey("loot") ? boss["loot"] as List<int> : new List<int>(),
							collection = boss.ContainsKey("collection") ? boss["collection"] as List<int> : new List<int>(),
						};
						bossInfos.Add(tinyBossInfo);
					}
				}
			}
		}

		//internal Dictionary<string, object> ConvertToDictionary()
		//{
		//	var dict = new Dictionary<string, object> {
		//		{ "key", Key },
		//		{ "progression", progression },
		//		{ "displayName", name },
		//		{ "npcIDs", new List<int>(npcIDs) },
		//		{ "downed", new Func<bool>(downed) },

		//		{ "isBoss", type.Equals(EntryType.Boss) },
		//		{ "isMiniboss", type.Equals(EntryType.MiniBoss) },
		//		{ "isEvent", type.Equals(EntryType.Event) },

		//		{ "spawnItem", new List<int>(spawnItem) },
		//		{ "loot", new List<int>(loot) },
		//		{ "collection", new List<int>(collection) }
		//	};

		//	return dict;
		//}

		public override void Unload()
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
