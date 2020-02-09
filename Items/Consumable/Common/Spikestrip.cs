using RiskOfSlimeRain.Core.ROREffects.Common;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable.Common
{
	public class Spikestrip : RORConsumableItem<SpikestripEffect>
	{
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Gel, 400);
			recipe.AddIngredient(ItemID.GrayPressurePlate, 20);
			recipe.AddIngredient(ItemID.SnowBrick, 250);
			recipe.AddIngredient(ItemID.CactusSword, 7);

			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}
