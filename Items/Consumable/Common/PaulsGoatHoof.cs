using RiskOfSlimeRain.Effects.Common;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable.Common
{
	public class PaulsGoatHoof : RORConsumableItem<PaulsGoatHoofEffect>
	{
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Bone, 999);
			recipe.AddIngredient(ItemID.FossilOre, 400);
			recipe.AddRecipeGroup("RoR:FastBoots", 8);
			recipe.AddIngredient(ItemID.AsphaltBlock, 350);
			recipe.AddIngredient(ItemID.Rally, 2);

			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}
