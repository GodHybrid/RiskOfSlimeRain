using RiskOfSlimeRain.Data.ROREffects.Common;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable.Common
{
	public class FireShield : RORConsumableItem<FireShieldEffect>
	{
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.ObsidianShield, 5);
			recipe.AddIngredient(ItemID.FlowerofFire, 3);
			recipe.AddIngredient(ItemID.HellstoneBrick, 666);
			recipe.AddIngredient(ItemID.Dynamite, 200);
			recipe.AddIngredient(ItemID.LargeAmber, 25);

			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}
