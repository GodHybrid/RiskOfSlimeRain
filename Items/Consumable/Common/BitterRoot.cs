using RiskOfSlimeRain.Effects.Common;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable.Common
{
	public class BitterRoot : RORConsumableItem<BitterRootEffect>
	{
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.LifeCrystal, 55);
			recipe.AddIngredient(ItemID.HealingPotion, 250);
			recipe.AddIngredient(ItemID.Salmon, 150);
			recipe.AddIngredient(ItemID.Blinkroot, 300);
			recipe.AddIngredient(ItemID.HoneyBlock, 400);
			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}
