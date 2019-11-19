using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items
{
	class PaulsGoatHoof : ModItem
	{

		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Run 20% faster");
		}

		public override void SetDefaults()
		{
			item.CloneDefaults(ItemID.LifeFruit);
            item.rare = ItemRarityID.White;
		}

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine line = new TooltipLine(mod, "Hoof", "A hoof from one of my many goats\nThinking it was cancerous, I went to the doctors and low-and-behold; it was");
            tooltips.Add(line);
            foreach (TooltipLine line2 in tooltips)
            {
                if (line2.Name == "Hoof")
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
            player.GetModPlayer<RORPlayer>().paulsGoatHooves++;
            
            return true;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Bone, 999);
            recipe.AddIngredient(ItemID.FossilOre, 400);
            recipe.AddRecipeGroup("RoR:FastBoots", 8);
            recipe.AddIngredient(ItemID.AsphaltBlock, 350);
            recipe.AddIngredient(ItemID.Rally, 2);
            
			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}
