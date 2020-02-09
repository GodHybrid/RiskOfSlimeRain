using RiskOfSlimeRain.Core.ROREffects.Common;
using RiskOfSlimeRain.Helpers;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable.Common
{
	public class BarbedWire : RORConsumableItem<BarbedWireEffect>
	{
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddRecipeGroupID(RecipeGroupID.IronBar, 60);
			recipe.AddIngredient(ItemID.ThornChakram, 3);
			recipe.AddIngredient(ItemID.SharkToothNecklace, 5);
			recipe.AddIngredient(ItemID.Wire, 210);

			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}
