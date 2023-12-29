using RiskOfSlimeRain.Core.ROREffects.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable.Common
{
	public class RustyKnife : RORConsumableItem<RustyKnifeEffect>
	{
		public override void SafeAddRecipes()
		{
			Recipe recipe = CreateRecipe(1);
			recipe.AddRecipeGroup(RORWorld.EvilWaterGroup, 60);
			recipe.AddIngredient(ItemID.ThrowingKnife, 900);
			recipe.AddIngredient(ItemID.SharkFin, 18);
			recipe.AddIngredient(ItemID.Swordfish, 5);
			recipe.Register();
		}
	}
}
