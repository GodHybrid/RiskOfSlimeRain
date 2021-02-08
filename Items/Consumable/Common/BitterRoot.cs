using RiskOfSlimeRain.Core.ROREffects.Common;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable.Common
{
	public class BitterRoot : RORConsumableItem<BitterRootEffect>
	{
		public override void SafeAddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.LifeCrystal, 8);
			recipe.AddIngredient(ItemID.HealingPotion, 50);
			recipe.AddIngredient(ItemID.Blinkroot, 200);
			recipe.AddIngredient(ItemID.HoneyBlock, 100);
			recipe.AddIngredient(ItemID.OrangeBloodroot, 2);
			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}
