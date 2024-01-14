using RiskOfSlimeRain.Core.ROREffects.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable.Common
{
	public class MortarTube : RORConsumableItem<MortarTubeEffect>
	{
		public override void SafeAddRecipes()
		{
			Recipe recipe = CreateRecipe(1);
			recipe.AddRecipeGroup(RecipeGroupID.IronBar, 300);
			recipe.AddIngredient(ItemID.Grenade, 90);
			recipe.AddIngredient(ItemID.FlareGun, 5);
			recipe.AddIngredient(ItemID.AmmoReservationPotion, 20);
			recipe.DisableDecraft();
			recipe.Register();
		}
	}
}
