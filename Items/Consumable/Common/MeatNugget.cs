using RiskOfSlimeRain.Core.ROREffects.Common;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable.Common
{
	public class MeatNugget : RORConsumableItem<MeatNuggetEffect>
	{
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Gel, 990);
			recipe.AddIngredient(ItemID.PinkGel, 170);
			recipe.AddRecipeGroup("RoR:EvilMaterial", 275);
			recipe.AddIngredient(ItemID.Bunny, 50);
			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}
