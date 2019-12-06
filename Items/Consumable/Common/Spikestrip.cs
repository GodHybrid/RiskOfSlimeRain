using RiskOfSlimeRain.Effects.Common;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable.Common
{
	public class Spikestrip : RORConsumableItem<SpikestripEffect>
	{
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddRecipeGroup("IronBar", 200);
			recipe.AddIngredient(ItemID.Gel, 700);
			recipe.AddIngredient(ItemID.GrayPressurePlate, 120);
			recipe.AddIngredient(ItemID.SnowBrick, 550);
			recipe.AddIngredient(ItemID.CactusSword, 30);

			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}
