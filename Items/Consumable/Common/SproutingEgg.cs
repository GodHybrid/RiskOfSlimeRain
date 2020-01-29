using RiskOfSlimeRain.Data.ROREffects.Common;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable.Common
{
	public class SproutingEgg : RORConsumableItem<SproutingEggEffect>
	{
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Vine, 100);
			recipe.AddIngredient(ItemID.RottenEgg, 300);
			recipe.AddIngredient(ItemID.Scorpion, 80);
			recipe.AddIngredient(ItemID.StrangeBrew, 300);
			recipe.AddIngredient(ItemID.Seed, 999);
			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}
