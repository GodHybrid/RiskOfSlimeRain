using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items
{
	class Medkit : ModItem
	{

		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Heal for 10 health 1.1 seconds after receiving damage");
		}

		public override void SetDefaults()
		{
			item.CloneDefaults(ItemID.LifeFruit);
            item.rare = ItemRarityID.White;
		}

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine line = new TooltipLine(mod, "Medkit", "Each Medkit should contain bandages, sterile dressings, soap,\nantiseptics, saline, gloves, scissors, aspirin, codeine, and an Epipen");
            tooltips.Add(line);
            foreach (TooltipLine line2 in tooltips)
            {
                if (line2.Name == "Medkit")
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
            player.GetModPlayer<RORPlayer>().medkits++;
            return true;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Safe, 5);
            recipe.AddIngredient(ItemID.Daybloom, 650);
            recipe.AddIngredient(ItemID.BottledHoney, 900);
            recipe.AddIngredient(ItemID.Sake, 1998);
            recipe.AddIngredient(ItemID.Bezoar, 10);
            recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}
