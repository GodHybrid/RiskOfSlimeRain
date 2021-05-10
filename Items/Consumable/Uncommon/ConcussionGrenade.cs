using RiskOfSlimeRain.Core.ROREffects.Uncommon;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable.Uncommon
{
	public class ConcussionGrenade : RORConsumableItem<ConcussionGrenadeEffect>
	{
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddRecipeGroup("RoR:Jellyfish", 20);
			recipe.AddIngredient(ItemID.TopazStaff, 4);
			recipe.AddIngredient(ItemID.CopperBar, 90);
			recipe.AddIngredient(ItemID.TeamBlockPink, 150);

			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}
