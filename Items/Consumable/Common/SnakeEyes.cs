using RiskOfSlimeRain.Core.ROREffects.Common;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable.Common
{
	public class SnakeEyes : RORConsumableItem<SnakeEyesEffect>
	{
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.RedCounterweight, 5);
			recipe.AddIngredient(ItemID.Ruby, 62);
			recipe.AddIngredient(ItemID.RichMahogany, 410);
			recipe.AddIngredient(ItemID.RedAcidDye, 10);

			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}
