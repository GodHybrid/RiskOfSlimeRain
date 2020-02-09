using RiskOfSlimeRain.Core.ROREffects.Common;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable.Common
{
	public class StickyBomb : RORConsumableItem<StickyBombEffect>
	{
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Grenade, 200);
			recipe.AddIngredient(ItemID.Gel, 400);
			recipe.AddIngredient(ItemID.Silk, 200);
			recipe.AddIngredient(ItemID.Timer1Second, 30);

			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}
