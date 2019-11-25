using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable
{
	public class LifeSavings : RORConsumableItem
	{
		public override void Initialize()
		{
			description = "Generate 1 copper every 3 seconds";
			flavorText = "hi im billy and heer is money for mom thanks";
		}

		public override void ApplyEffect(RORPlayer mPlayer)
		{
			mPlayer.savings++;
			mPlayer.piggyBankTimer = 180 / mPlayer.savings + 1;
		}

		public override void ResetEffect(RORPlayer mPlayer)
		{
			mPlayer.savings = 0;
			mPlayer.piggyBankTimer = -1;
		}

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
