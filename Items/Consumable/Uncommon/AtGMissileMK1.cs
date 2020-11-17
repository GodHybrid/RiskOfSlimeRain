using RiskOfSlimeRain.Core.ROREffects.Uncommon;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable.Uncommon
{
	public class AtGMissileMK1 : RORConsumableItem<AtGMissileMK1Effect>
	{
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.DirtBlock, 60);

			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}
