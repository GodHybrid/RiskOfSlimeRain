using RiskOfSlimeRain.Core.ROREffects.Common;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable.Common
{
	public class LifeSavings : RORConsumableItem<LifeSavingsEffect>
	{
		public override void SafeAddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.PiggyBank, 2);
			recipe.AddIngredient(ItemID.GoldCoin, 50);
			recipe.AddIngredient(ItemID.ClayBlock, 600);
			recipe.AddIngredient(ItemID.GoldenCarp, 10);

			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}
