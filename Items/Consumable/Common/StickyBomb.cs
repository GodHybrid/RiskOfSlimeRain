using RiskOfSlimeRain.Core.ROREffects.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable.Common
{
	public class StickyBomb : RORConsumableItem<StickyBombEffect>
	{
		public override void SafeAddRecipes()
		{
			Recipe recipe = CreateRecipe(1);
			recipe.AddIngredient(ItemID.Grenade, 200);
			recipe.AddIngredient(ItemID.Gel, 400);
			recipe.AddIngredient(ItemID.Silk, 200);
			recipe.AddIngredient(ItemID.Timer1Second, 30);
			recipe.DisableDecraft();
			recipe.Register();
		}
	}
}
