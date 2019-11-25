using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable
{
	public class MeatNugget : RORConsumableItem
	{
		public override void Initialize()
		{
			description = "Upon use enemies will have an 8% chance to drop two meat nuggets\nEach meat nugget recovers 6 health";
			flavorText = "MM. Delicious\nJust kidding, it's awful";
		}

		public override void ApplyEffect(RORPlayer mPlayer)
		{
			mPlayer.meatNuggets++;
		}

		public override void ResetEffect(RORPlayer mPlayer)
		{
			mPlayer.meatNuggets = 0;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Gel, 9990);
			recipe.AddIngredient(ItemID.PinkGel, 400);
			recipe.AddRecipeGroup("RoR:EvilMaterial", 999);
			recipe.AddIngredient(ItemID.Bunny, 100);
			recipe.AddIngredient(ItemID.HealingPotion, 250);
			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}
