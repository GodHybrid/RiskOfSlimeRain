﻿using RiskOfSlimeRain.Effects.Common;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable.Common
{
	public class Medkit : RORConsumableItem<MedkitEffect>
	{
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