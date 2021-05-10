using RiskOfSlimeRain.Core.ROREffects.Uncommon;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable.Uncommon
{
	class ChargefieldGenerator : RORConsumableItem<ChargefieldGeneratorEffect>
	{
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddRecipeGroup("IronBar", 1);

			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}
