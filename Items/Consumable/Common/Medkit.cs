using RiskOfSlimeRain.Core.ROREffects.Common;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable.Common
{
	public class Medkit : RORConsumableItem<MedkitEffect>
	{
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Safe, 1);
			recipe.AddIngredient(ItemID.Daybloom, 650);
			recipe.AddIngredient(ItemID.BottledHoney, 260);
			recipe.AddIngredient(ItemID.Bezoar, 1);

			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}
