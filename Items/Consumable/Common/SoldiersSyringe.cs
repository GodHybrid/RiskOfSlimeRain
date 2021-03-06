﻿using RiskOfSlimeRain.Core.ROREffects.Common;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable.Common
{
	public class SoldiersSyringe : RORConsumableItem<SoldiersSyringeEffect>
	{
		public override void SafeAddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddRecipeGroup("RoR:EvilMushrooms", 240);
			recipe.AddIngredient(ItemID.WineGlass, 100);
			recipe.AddIngredient(ItemID.FeralClaws, 2);
			recipe.AddIngredient(ItemID.SwiftnessPotion, 40);

			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}
