using RiskOfSlimeRain.Core.ROREffects.Common;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable.Common
{
	public class HermitsScarf : RORConsumableItem<HermitsScarfEffect>
	{
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Silk, 999);
			recipe.AddIngredient(ItemID.SlushBlock, 900);
			recipe.AddIngredient(ItemID.Feather, 350);
			recipe.AddIngredient(ItemID.Gi, 5);
			recipe.AddIngredient(ItemID.TrapsightPotion, 200);

			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}
