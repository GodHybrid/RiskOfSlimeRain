using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable
{
	public class SoldiersSyringe : RORConsumableItem
	{
		public override void Initialize()
		{
			description = "Increase attack speed by 15%";
			flavorText = "Should help multi-purpose requirements needed of soldiers\nContains vaccinations, antibiotics, pain killers, steroids, heroine, gasoline...";
		}

		public override bool CanUse(RORPlayer mPlayer)
		{
			return mPlayer.soldiersSyringes < 13;
		}

		public override void ApplyEffect(RORPlayer mPlayer)
		{
			mPlayer.soldiersSyringes++;
		}

		public override void ResetEffect(RORPlayer mPlayer)
		{
			mPlayer.soldiersSyringes = 0;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddRecipeGroup("IronBar", 230);
			recipe.AddRecipeGroup("RoR:EvilMushrooms", 600);
			recipe.AddIngredient(ItemID.WineGlass, 150);
			recipe.AddIngredient(ItemID.FeralClaws, 5);
			recipe.AddIngredient(ItemID.SwiftnessPotion, 200);

			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}
