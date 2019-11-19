using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items
{
	class SproutingEgg : ModItem
	{

		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Permanently increases health regeneration by 2.4 health per second when out of combat");
		}

		public override void SetDefaults()
		{
			item.CloneDefaults(ItemID.LifeFruit);
            item.rare = ItemRarityID.White;
		}

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine line = new TooltipLine(mod, "Sprout", "This egg seems to be somewhere between hatching and dying\nI can't bring it to myself to cook it alive");
            tooltips.Add(line);
            foreach (TooltipLine line2 in tooltips)
            {
                if (line2.Name == "Sprout")
                {
                    line.overrideColor = Color.FloralWhite;
                }
            }
        }

        public override bool CanUseItem(Player player)
        {
            return true;
        }

        public override bool UseItem(Player player)
		{
            player.GetModPlayer<RORPlayer>().sproutingEggs++;
            return true;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Vine, 100);
            recipe.AddIngredient(ItemID.RottenEgg, 300);
            recipe.AddIngredient(ItemID.Scorpion, 80);
            recipe.AddIngredient(ItemID.StrangeBrew, 300);
            recipe.AddIngredient(ItemID.Seed, 999);
			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}
