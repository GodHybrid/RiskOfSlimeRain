using RiskOfSlimeRain.Core.ROREffects.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable.Common
{
	public class Taser : RORConsumableItem<TaserEffect>
	{
		public override void SafeAddRecipes()
		{
			Recipe recipe = CreateRecipe(1);
			recipe.AddRecipeGroup("RoR:Jellyfish", 20);
			recipe.AddIngredient(ItemID.TopazStaff, 4);
			recipe.AddIngredient(ItemID.CopperBar, 90);
			recipe.AddIngredient(ItemID.TeamBlockPink, 150);
			recipe.Register();
		}
	}
}
