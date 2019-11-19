using Microsoft.Xna.Framework;
 using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items
{
	class Warbanner : ModItem
	{

		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Chance to drop an empowering banner");
		}

		public override void SetDefaults()
		{
			item.CloneDefaults(ItemID.LifeFruit);
            item.rare = ItemRarityID.White;
		}

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine line = new TooltipLine(mod, "Banner", "Very very valuable\nDon't drop it; it's worth more than you");
            tooltips.Add(line);
            foreach (TooltipLine line2 in tooltips)
            {
                if (line2.Name == "Banner")
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
            player.GetModPlayer<RORPlayer>().warbanners++;
            return true;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.FieryGreatsword, 2);
            recipe.AddIngredient(ItemID.HellwingBow, 1);
            recipe.AddIngredient(ItemID.BorealWood, 505);
            recipe.AddIngredient(ItemID.TatteredCloth, 60);
            recipe.AddIngredient(ItemID.InfernoPotion, 50);
            
			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}
