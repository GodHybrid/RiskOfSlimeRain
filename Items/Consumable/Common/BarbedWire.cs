using RiskOfSlimeRain.Core.ROREffects.Common;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable.Common
{
	public class BarbedWire : RORConsumableItem<BarbedWireEffect>
	{
		public override void SafeAddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddRecipeGroup("IronBar", 60);
			recipe.AddIngredient(ItemID.ThornChakram, 3);
			recipe.AddIngredient(ItemID.SharkToothNecklace, 5);
			recipe.AddIngredient(ItemID.Wire, 210);

			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}
