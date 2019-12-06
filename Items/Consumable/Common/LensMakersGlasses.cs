using RiskOfSlimeRain.Effects.Common;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable.Common
{
	public class LensMakersGlasses : RORConsumableItem<LensmakersGlassesEffect>
	{
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Lens, 500);
			recipe.AddIngredient(ItemID.BlackLens, 40);
			recipe.AddIngredient(ItemID.Ruby, 300);
			recipe.AddIngredient(ItemID.MeteoriteBrick, 350);
			recipe.AddIngredient(ItemID.CorruptHardenedSand, 250);

			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}
