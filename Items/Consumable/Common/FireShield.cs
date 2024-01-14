using RiskOfSlimeRain.Core.ROREffects.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable.Common
{
	public class FireShield : RORConsumableItem<FireShieldEffect>
	{
		public override void SafeAddRecipes()
		{
			Recipe recipe = CreateRecipe(1);
			recipe.AddIngredient(ItemID.ObsidianShield, 2);
			recipe.AddIngredient(ItemID.FlowerofFire, 1);
			recipe.AddIngredient(ItemID.HellstoneBrick, 666);
			recipe.AddIngredient(ItemID.LargeAmber, 5);
			recipe.DisableDecraft();
			recipe.Register();
		}
	}
}
