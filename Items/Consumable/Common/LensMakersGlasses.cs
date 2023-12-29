using RiskOfSlimeRain.Core.ROREffects.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable.Common
{
	public class LensMakersGlasses : RORConsumableItem<LensmakersGlassesEffect>
	{
		public override void SafeAddRecipes()
		{
			Recipe recipe = CreateRecipe(1);
			recipe.AddIngredient(ItemID.Lens, 140);
			recipe.AddIngredient(ItemID.BlackLens, 16);
			recipe.AddIngredient(ItemID.Ruby, 70);
			recipe.AddIngredient(ItemID.MeteoriteBrick, 270);
			recipe.Register();
		}
	}
}
