using RiskOfSlimeRain.Effects.Common;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable.Common
{
	public class SoldiersSyringe : RORConsumableItem<SoldiersSyringeEffect>
	{
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
