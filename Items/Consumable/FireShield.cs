using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable
{
	public class FireShield : RORConsumableItem
	{
		public override void Initialize()
		{
			description = "After being hit for 10% of your max health - explode, dealing 200 damage";
			flavorText = "The thing is only half-done, but it will do the job\nPLEASE handle with care!";
		}

		public override void ApplyEffect(RORPlayer mPlayer)
		{
			mPlayer.fireShields++;
		}

		public override void ResetEffect(RORPlayer mPlayer)
		{
			mPlayer.fireShields = 0;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.ObsidianShield, 5);
			recipe.AddIngredient(ItemID.FlowerofFire, 3);
			recipe.AddIngredient(ItemID.HellstoneBrick, 666);
			recipe.AddIngredient(ItemID.Dynamite, 200);
			recipe.AddIngredient(ItemID.LargeAmber, 25);

			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}
