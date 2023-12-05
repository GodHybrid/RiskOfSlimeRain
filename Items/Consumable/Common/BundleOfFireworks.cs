using RiskOfSlimeRain.Core.ROREffects.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable.Common
{
	public class BundleOfFireworks : RORConsumableItem<BundleOfFireworksEffect>
	{
		public override void SafeAddRecipes()
		{
			Recipe recipe = CreateRecipe(1);
			recipe.AddIngredient(ItemID.RedRocket, 450);
			recipe.AddIngredient(ItemID.BlueRocket, 450);
			recipe.AddIngredient(ItemID.RopeCoil, 55);
			recipe.AddIngredient(ItemID.FireworkFountain, 10);
			recipe.Register();
		}
	}
}
