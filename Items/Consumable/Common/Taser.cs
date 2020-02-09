using RiskOfSlimeRain.Core.ROREffects.Common;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable.Common
{
	public class Taser : RORConsumableItem<TaserEffect>
	{
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.TopazStaff, 4);
			recipe.AddIngredient(ItemID.CopperBar, 90);
			recipe.AddIngredient(ItemID.TeamBlockPink, 150);
			recipe.AddRecipeGroup("RoR:Jellyfish", 20);

			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}
