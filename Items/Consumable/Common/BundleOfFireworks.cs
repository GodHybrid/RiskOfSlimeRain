using RiskOfSlimeRain.Core.ROREffects.Common;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable.Common
{
	public class BundleOfFireworks : RORConsumableItem<BundleOfFireworksEffect>
	{
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.RedRocket, 450);
			recipe.AddIngredient(ItemID.BlueRocket, 450);
			recipe.AddIngredient(ItemID.RopeCoil, 55);
			recipe.AddIngredient(ItemID.FireworkFountain, 10);

			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}
