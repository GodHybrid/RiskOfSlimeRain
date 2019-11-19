using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items
{
	class Spikestrip : ModItem
	{

		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Drop spikestrips on hit, slowing enemies by 20%");
		}

		public override void SetDefaults()
		{
			item.CloneDefaults(ItemID.LifeFruit);
            item.rare = ItemRarityID.White;
		}

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            TooltipLine line = new TooltipLine(mod, "Spikestrip", "The doctors say I don't have much time left\nSince you're in the force now and all, I felt obligated to return it to you");
            tooltips.Add(line);
            foreach (TooltipLine line2 in tooltips)
            {
                if (line2.Name == "Spikestrip")
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
            player.GetModPlayer<RORPlayer>().spikestrips++;
            return true;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
            recipe.AddRecipeGroup("IronBar", 200);
            recipe.AddIngredient(ItemID.Gel, 700);
            recipe.AddIngredient(ItemID.GrayPressurePlate, 120);
            recipe.AddIngredient(ItemID.SnowBrick, 550);
            recipe.AddIngredient(ItemID.CactusSword, 30);
            
			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}
