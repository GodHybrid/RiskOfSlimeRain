using RiskOfSlimeRain.Core.ROREffects.Common;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable.Common
{
	public class Warbanner : RORConsumableItem<WarbannerEffect>
	{
		public override void SafeAddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.FieryGreatsword, 2);
			recipe.AddIngredient(ItemID.BorealWood, 505);
			recipe.AddIngredient(ItemID.TatteredCloth, 60);
			recipe.AddIngredient(ItemID.InfernoPotion, 9);

			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}
