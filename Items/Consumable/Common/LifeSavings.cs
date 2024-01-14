using RiskOfSlimeRain.Core.ROREffects.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable.Common
{
	public class LifeSavings : RORConsumableItem<LifeSavingsEffect>
	{
		public override void SafeAddRecipes()
		{
			Recipe recipe = CreateRecipe(1);
			recipe.AddIngredient(ItemID.PiggyBank, 2);
			recipe.AddIngredient(ItemID.GoldCoin, 50);
			recipe.AddIngredient(ItemID.ClayBlock, 600);
			recipe.AddIngredient(ItemID.GoldenCarp, 10);
			recipe.DisableDecraft();
			recipe.Register();
		}
	}
}
