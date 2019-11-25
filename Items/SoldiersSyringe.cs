using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items
{
	class SoldiersSyringe : ModItem
	{
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Increase attack speed by 15%");
		}

		public override void SetDefaults()
		{
			item.CloneDefaults(ItemID.LifeFruit);
			item.rare = ItemRarityID.White;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			TooltipLine line = new TooltipLine(mod, "Syringe", "Should help multi-purpose requirements needed of soldiers\nContains vaccinations, antibiotics, pain killers, steroids, heroine, gasoline...");
			tooltips.Add(line);
			foreach (TooltipLine line2 in tooltips)
			{
				if (line2.Name == "Syringe")
				{
					line.overrideColor = Color.FloralWhite;
				}
			}
		}

		public override bool CanUseItem(Player player)
		{
			if (player.GetModPlayer<RORPlayer>().soldiersSyringes < 13) return true;
			return false;
		}

		public override bool UseItem(Player player)
		{
			player.GetModPlayer<RORPlayer>().soldiersSyringes++;
			return true;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddRecipeGroup("IronBar", 230);
			recipe.AddRecipeGroup("RoR:EvilMushrooms", 600);
			recipe.AddIngredient(ItemID.WineGlass, 150);
			recipe.AddIngredient(ItemID.FeralClaws, 5);
			recipe.AddIngredient(ItemID.SwiftnessPotion, 200);

			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}
