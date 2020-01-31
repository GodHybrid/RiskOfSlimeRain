using RiskOfSlimeRain.Core.ROREffects.Common;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable.Common
{
	public class LifeSavings : RORConsumableItem<LifeSavingsEffect>
	{
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.PiggyBank, 5);
			recipe.AddIngredient(ItemID.GoldCoin, 150);
			recipe.AddIngredient(ItemID.ClayBlock, 600);
			recipe.AddIngredient(ItemID.GoldenCarp, 30);
			recipe.AddIngredient(ItemID.Bacon, 6);

			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}
