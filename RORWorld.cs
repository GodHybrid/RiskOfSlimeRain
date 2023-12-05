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
		}

		public override void NetReceive(BinaryReader reader)
		{
			BitsByte flags = reader.ReadByte();
			downedMagmaWorm = flags[0];

			NPCLootManager.NetReceive(reader, true);
			NPCLootManager.NetReceive(reader, false);
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

		public override bool HijackSendData(int whoAmI, int msgType, int remoteClient, int ignoreClient, NetworkText text, int number, float number2, float number3, float number4, int number5, int number6, int number7)
		{
			SpawnedFromStatuePacket.HijackSendData(msgType, number);
			return base.HijackSendData(whoAmI, msgType, remoteClient, ignoreClient, text, number, number2, number3, number4, number5, number6, number7);
		}
	}
}
