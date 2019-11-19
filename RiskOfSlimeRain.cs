using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace RiskOfSlimeRain
{
	public class RiskOfSlimeRain : Mod
	{
		public RiskOfSlimeRain()
		{
		}

        public override void AddRecipeGroups()
        {
            RecipeGroup HMTier3Bar_Group = new RecipeGroup(() => Language.GetTextValue("LegacyMisc.37") + " Tier 3 Hardmode bar", new int[]
            {
                ItemID.AdamantiteBar,
                ItemID.TitaniumBar
            });
            RecipeGroup.RegisterGroup("RoR:Tier3HMBar", HMTier3Bar_Group);

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
    }
}