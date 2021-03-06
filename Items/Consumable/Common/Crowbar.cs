﻿using RiskOfSlimeRain.Core.ROREffects.Common;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable.Common
{
	public class Crowbar : RORConsumableItem<CrowbarEffect>
	{
		public override void SafeAddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddRecipeGroup("RoR:SilvTungBar", 65);
			recipe.AddIngredient(ItemID.BreathingReed, 3);
			recipe.AddIngredient(ItemID.Wrench, 6);
			recipe.AddIngredient(ItemID.BlackDye, 30);

			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}
