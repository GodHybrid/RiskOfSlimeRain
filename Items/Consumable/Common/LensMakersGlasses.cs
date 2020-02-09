using RiskOfSlimeRain.Core.ROREffects.Common;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable.Common
{
	public class LensMakersGlasses : RORConsumableItem<LensmakersGlassesEffect>
	{
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Lens, 140);
			recipe.AddIngredient(ItemID.BlackLens, 16);
			recipe.AddIngredient(ItemID.Ruby, 70);
			recipe.AddIngredient(ItemID.MeteoriteBrick, 270);

			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}
