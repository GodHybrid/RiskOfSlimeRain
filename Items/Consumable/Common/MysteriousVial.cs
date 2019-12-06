using RiskOfSlimeRain.Effects.Common;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable.Common
{
	public class MysteriousVial : RORConsumableItem<MysteriousVialEffect>
	{
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Bottle, 1);
			recipe.AddIngredient(ItemID.RegenerationPotion, 400);
			recipe.AddIngredient(ItemID.NeonTetra, 250);
			recipe.AddIngredient(ItemID.DaybloomSeeds, 600);
			recipe.AddIngredient(ItemID.Damselfish, 200);
			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}
