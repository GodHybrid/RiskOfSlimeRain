using RiskOfSlimeRain.Core.ItemSpawning.ChestSpawning;
using RiskOfSlimeRain.Core.ItemSpawning.NPCSpawning;
using RiskOfSlimeRain.Core.Warbanners;
using RiskOfSlimeRain.Network.NPCs;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace RiskOfSlimeRain
{
	public class RORWorld : ModSystem
	{
		//public static bool rorMode;
		public static bool downedMagmaWorm = false;

		public static LocalizedText RecipeGroupAnyText { get; private set; }
		public static LocalizedText ChestText { get; private set; }
		public static LocalizedText JellyfishText { get; private set; }
		public static LocalizedText EvilWaterText { get; private set; }
		public static LocalizedText EvilMaterialText { get; private set; }
		public static LocalizedText BasicBootsText { get; private set; }
		public static LocalizedText EvilMushroomText { get; private set; }

		public override void OnModLoad()
		{
			string category = $"RecipeGroups.";
			RecipeGroupAnyText ??= Language.GetOrRegister(Mod.GetLocalizationKey($"{category}Any"));
			ChestText ??= Language.GetOrRegister(Mod.GetLocalizationKey($"{category}Chest"));
			JellyfishText ??= Language.GetOrRegister(Mod.GetLocalizationKey($"{category}Jellyfish"));
			EvilWaterText ??= Language.GetOrRegister(Mod.GetLocalizationKey($"{category}EvilWater"));
			EvilMaterialText ??= Language.GetOrRegister(Mod.GetLocalizationKey($"{category}EvilMaterial"));
			BasicBootsText ??= Language.GetOrRegister(Mod.GetLocalizationKey($"{category}BasicBoots"));
			EvilMushroomText ??= Language.GetOrRegister(Mod.GetLocalizationKey($"{category}EvilMushroom"));
		}

		public override void ClearWorld()
		{
			//rorMode = false;
			downedMagmaWorm = false;
			ChestManager.Init();
		}

		public override void NetSend(BinaryWriter writer)
		{
			BitsByte flags = new BitsByte();
			flags[0] = downedMagmaWorm;
			writer.Write(flags);

			NPCLootManager.NetSend(writer, true);
			NPCLootManager.NetSend(writer, false);

			//Warbanner backend is all serverside, so clients don't need to know about that
			ChestManager.NetSend(writer);
		}

		public override void NetReceive(BinaryReader reader)
		{
			BitsByte flags = reader.ReadByte();
			downedMagmaWorm = flags[0];

			NPCLootManager.NetReceive(reader, true);
			NPCLootManager.NetReceive(reader, false);

			ChestManager.NetReceive(reader);
		}

		public override void LoadWorldData(TagCompound tag)
		{
			downedMagmaWorm = tag.GetBool("downedMagmaWorm");

			WarbannerManager.Load(tag);
			ChestManager.Load(tag);
			NPCLootManager.Load(tag);
		}

		public override void SaveWorldData(TagCompound tag)
		{
			tag.Add("downedMagmaWorm", downedMagmaWorm);

			WarbannerManager.Save(tag);
			ChestManager.Save(tag);
			NPCLootManager.Save(tag);
		}

		public override void PostWorldGen()
		{
			ChestManager.AddItemsToChests();
		}

		public override void PostUpdateWorld()
		{
			WarbannerManager.TrySpawnWarbanners();
			SpawnedFromStatuePacket.SendSpawnedFromStatues();
		}

		public static int HMTier3BarGroup { get; private set; }
		public static int SilvTungBarGroup { get; private set; }
		public static int GoldPlatBarGroup { get; private set; }
		public static int ChestGroup { get; private set; }
		public static int JellyfishGroup { get; private set; }
		public static int EvilWaterGroup { get; private set; }
		public static int EvilMaterialGroup { get; private set; }
		public static int BasicBootsGroup { get; private set; }
		public static int EvilMushroomGroup { get; private set; }

		public override void AddRecipeGroups()
		{
			string any = Language.GetTextValue("LegacyMisc.37");
			HMTier3BarGroup = RecipeGroup.RegisterGroup(nameof(ItemID.AdamantiteBar), new RecipeGroup(() => RecipeGroupAnyText.Format(any, Lang.GetItemNameValue(ItemID.AdamantiteBar)), new int[]
			{
				ItemID.AdamantiteBar,
				ItemID.TitaniumBar
			}));

			SilvTungBarGroup = RecipeGroup.RegisterGroup(nameof(ItemID.SilverBar), new RecipeGroup(() => RecipeGroupAnyText.Format(any, Lang.GetItemNameValue(ItemID.SilverBar)), new int[]
			{
				ItemID.SilverBar,
				ItemID.TungstenBar
			}));

			GoldPlatBarGroup = RecipeGroup.RegisterGroup(nameof(ItemID.GoldBar), new RecipeGroup(() => RecipeGroupAnyText.Format(any, Lang.GetItemNameValue(ItemID.GoldBar)), new int[]
			{
				ItemID.GoldBar,
				ItemID.PlatinumBar
			}));

			ChestGroup = RecipeGroup.RegisterGroup(nameof(ItemID.Chest), new RecipeGroup(() => RecipeGroupAnyText.Format(any, ChestText), new int[]
			{
				#region Chest_List
				ItemID.Chest,
				ItemID.BlueDungeonChest,
				ItemID.BoneChest,
				ItemID.BorealWoodChest,
				ItemID.CactusChest,
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
			}));

			JellyfishGroup = RecipeGroup.RegisterGroup(nameof(ItemID.BlueJellyfish), new RecipeGroup(() => RecipeGroupAnyText.Format(any, JellyfishText), new int[]
			{
				ItemID.BlueJellyfish,
				ItemID.GreenJellyfish,
				ItemID.PinkJellyfish
			}));

			EvilWaterGroup = RecipeGroup.RegisterGroup(nameof(ItemID.UnholyWater), new RecipeGroup(() => RecipeGroupAnyText.Format(any, EvilWaterText), new int[]
			{
				ItemID.UnholyWater,
				ItemID.BloodWater
			}));

			EvilMaterialGroup = RecipeGroup.RegisterGroup(nameof(ItemID.RottenChunk), new RecipeGroup(() => RecipeGroupAnyText.Format(any, EvilMaterialText), new int[]
			{
				ItemID.RottenChunk,
				ItemID.Vertebrae
			}));

			BasicBootsGroup = RecipeGroup.RegisterGroup(nameof(ItemID.HermesBoots), new RecipeGroup(() => RecipeGroupAnyText.Format(any, BasicBootsText), new int[]
			{
				ItemID.HermesBoots,
				ItemID.FlurryBoots,
				ItemID.SailfishBoots,
				ItemID.SandBoots
			}));

			EvilMushroomGroup = RecipeGroup.RegisterGroup(nameof(ItemID.VileMushroom), new RecipeGroup(() => RecipeGroupAnyText.Format(any, EvilMushroomText), new int[]
			{
				ItemID.VileMushroom,
				ItemID.ViciousMushroom
			}));
		}

		public override bool HijackSendData(int whoAmI, int msgType, int remoteClient, int ignoreClient, NetworkText text, int number, float number2, float number3, float number4, int number5, int number6, int number7)
		{
			SpawnedFromStatuePacket.HijackSendData(msgType, number);
			return base.HijackSendData(whoAmI, msgType, remoteClient, ignoreClient, text, number, number2, number3, number4, number5, number6, number7);
		}
	}
}
