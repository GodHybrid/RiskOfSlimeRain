using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items
{
	class MeatNugget : ModItem
	{

		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Upon use enemies will have an 8% chance to drop two meat nuggets\nEach meat nugget recovers 6 health");
		}

		public override void SetDefaults()
		{
			item.CloneDefaults(ItemID.LifeFruit);
            item.rare = ItemRarityID.White;
		}

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine line = new TooltipLine(mod, "Nugget", "MM. Delicious\nJust kidding, it's awful");
            tooltips.Add(line);
            foreach (TooltipLine line2 in tooltips)
            {
                if (line2.Name == "Nugget")
                {
                    line.overrideColor = Color.FloralWhite;
                }
            }
        }

        public override bool CanUseItem(Player player)
        {
            return true; // player.GetModPlayer<RORPlayer>().bitterRootIncrease < player.statLifeMax *3;
        }

        public override bool UseItem(Player player)
		{
            player.GetModPlayer<RORPlayer>().meatNuggets++;
            return true;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Gel, 9990);
            recipe.AddIngredient(ItemID.PinkGel, 400);
            recipe.AddRecipeGroup("RoR:EvilMaterial", 999);
            recipe.AddIngredient(ItemID.Bunny, 100);
            recipe.AddIngredient(ItemID.HealingPotion, 250);
			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}
