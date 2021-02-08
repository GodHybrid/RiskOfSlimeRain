using RiskOfSlimeRain.Core.ROREffects.Common;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable.Common
{
	public class PaulsGoatHoof : RORConsumableItem<PaulsGoatHoofEffect>
	{
		public override void SafeAddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddRecipeGroup("RoR:FastBoots", 1);
			recipe.AddIngredient(ItemID.Bone, 355);
			recipe.AddIngredient(ItemID.FossilOre, 280);
			recipe.AddIngredient(ItemID.Rally, 3);

			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}
