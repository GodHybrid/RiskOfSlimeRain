using RiskOfSlimeRain.Core.ROREffects.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable.Common
{
	public class MeatNugget : RORConsumableItem<MeatNuggetEffect>
	{
		public override void SafeAddRecipes()
		{
			Recipe recipe = CreateRecipe(1);
			recipe.AddRecipeGroup(RORWorld.EvilMaterialGroup, 275);
			recipe.AddIngredient(ItemID.Gel, 990);
			recipe.AddIngredient(ItemID.PinkGel, 170);
			recipe.AddIngredient(ItemID.Bunny, 50);
			recipe.DisableDecraft();
			recipe.Register();
		}
	}
}
