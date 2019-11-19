using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Achievements;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items
{
    class Nullifier : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Every upgrade you ever got...\nGone with the wind");
        }

        public override void SetDefaults()
        {
            item.CloneDefaults(ItemID.LifeFruit);
            item.rare = ItemRarityID.Red;
            item.UseSound = new Terraria.Audio.LegacySoundStyle(SoundID.Shatter, 0);
        }

        public override bool CanUseItem(Player player)
        {
            return true;
        }

        public override bool UseItem(Player player)
        {
            RORPlayer TP = player.GetModPlayer<RORPlayer>();
            player.statLifeMax2 -= player.GetModPlayer<RORPlayer>().bitterRootIncrease;
            TP.bitterRoots = 0;
            TP.bitterRootIncrease = 0;
            TP.bustlingFungi = 0;
            TP.bustlingFungusHeals = 0;
            TP.fungalRadius = 0;
            TP.fungalDefense = false;
            TP.meatNuggets = 0;
            TP.monsterTeeth = 0;
            TP.medkits = 0;
            TP.medkitTimer = -1;
            TP.mysteriousVials = 0;
            TP.sproutingEggs = 0;
            TP.sproutingEggTimer = -1;
            TP.scarfs = 0;
            TP.lensMakersGlasses = 0;
            TP.fireShields = 0;
            TP.spikestrips = 0;
            TP.tasers = 0;
            TP.warbanners = 0;
            RORWorld.pos.Clear();
            RORWorld.radius.Clear();
            TP.savings = 0;
            TP.piggyBankTimer = -1;
            TP.paulsGoatHooves = 0;
            TP.snakeEyesDice = 0;
            TP.snakeEyesDiceIncrease = 0;
            TP.snakeEyesDiceReady = false;
            TP.soldiersSyringes = 0;
            TP.barbedWires = 0;
            TP.wireTimer = -1;
            TP.crowbars = 0;
            TP.gasCanisters = 0;
            TP.stompers = 0;
            return true;
        }
    }
}
