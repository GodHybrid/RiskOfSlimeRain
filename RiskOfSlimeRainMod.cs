using RiskOfSlimeRain.Core.NPCEffects;
using RiskOfSlimeRain.Core.ROREffects;
using RiskOfSlimeRain.Core.Warbanners;
using RiskOfSlimeRain.Effects;
using System.Collections.Generic;
using System.IO;
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
