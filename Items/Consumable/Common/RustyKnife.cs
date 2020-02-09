using RiskOfSlimeRain.Core.ROREffects.Common;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable.Common
{
	public class RustyKnife : RORConsumableItem<RustyKnifeEffect>
	{
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.ThrowingKnife, 900);
			recipe.AddRecipeGroup("RoR:EvilWater", 60);
			recipe.AddIngredient(ItemID.SharkFin, 18);
			recipe.AddIngredient(ItemID.Swordfish, 5);

			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}
