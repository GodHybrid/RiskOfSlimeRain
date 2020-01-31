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
			recipe.AddIngredient(ItemID.Ruby, 172);
			recipe.AddIngredient(ItemID.RichMahogany, 500);
			recipe.AddIngredient(ItemID.RedAcidDye, 10);
			recipe.AddIngredient(ItemID.SharkFin, 150);

			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}
