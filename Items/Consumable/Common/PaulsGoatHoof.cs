using RiskOfSlimeRain.Core.ROREffects.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable.Common
{
	public class PaulsGoatHoof : RORConsumableItem<PaulsGoatHoofEffect>
	{
		public override void SafeAddRecipes()
		{
			Recipe recipe = CreateRecipe(1);
			recipe.AddRecipeGroup(RORWorld.BasicBootsGroup, 1);
			recipe.AddIngredient(ItemID.Bone, 355);
			recipe.AddIngredient(ItemID.FossilOre, 280);
			recipe.AddIngredient(ItemID.Rally, 3);
			recipe.Register();
		}
	}
}
