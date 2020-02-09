using RiskOfSlimeRain.Core.ROREffects.Common;
using RiskOfSlimeRain.Helpers;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable.Common
{
	public class MortarTube : RORConsumableItem<MortarTubeEffect>
	{
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddRecipeGroupID(RecipeGroupID.IronBar, 300);
			recipe.AddIngredient(ItemID.Grenade, 90);
			recipe.AddIngredient(ItemID.FlareGun, 5);
			recipe.AddIngredient(ItemID.AmmoReservationPotion, 20);

			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}
