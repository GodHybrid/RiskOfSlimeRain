using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable
{
	public class SnakeEyes : RORConsumableItem
	{
		public override void Initialize()
		{
			description = "Increase crit chance by 6% for each time you're in peril, up to 4 times";
			flavorText = "You dirty----------er\nYou KNEW I had to win to pay off my debts";
		}

		public override void ApplyEffect(RORPlayer mPlayer)
		{
			mPlayer.snakeEyesDice++;
		}

		public override void ResetEffect(RORPlayer mPlayer)
		{
			mPlayer.snakeEyesDice = 0;
			mPlayer.snakeEyesDiceIncrease = 0;
			mPlayer.snakeEyesDiceReady = false;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.RedCounterweight, 5);
			recipe.AddIngredient(ItemID.Ruby, 172);
			recipe.AddIngredient(ItemID.RichMahogany, 500);
			recipe.AddIngredient(ItemID.RedAcidDye, 10);
			recipe.AddIngredient(ItemID.SharkFin, 150);

			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}
