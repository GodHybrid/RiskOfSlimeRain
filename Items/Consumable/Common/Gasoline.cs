using RiskOfSlimeRain.Core.ROREffects.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable.Common
{
	public class Gasoline : RORConsumableItem<GasolineEffect>
	{
		public override void SafeAddRecipes()
		{
			Recipe recipe = CreateRecipe(1);
			recipe.AddIngredient(ItemID.FireblossomSeeds, 600);
			recipe.AddIngredient(ItemID.Gel, 800);
			recipe.AddIngredient(ItemID.Hellstone, 260);
			recipe.AddIngredient(ItemID.TinCan, 45);
			recipe.DisableDecraft();
			recipe.Register();
		}
	}
}
