using RiskOfSlimeRain.Core.ROREffects.Uncommon;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable.Uncommon
{
	class ToughTimes : RORConsumableItem<ToughTimesEffect>
	{
		public override void AddRecipes()
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
