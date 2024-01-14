using RiskOfSlimeRain.Core.ROREffects.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable.Common
{
	public class MysteriousVial : RORConsumableItem<MysteriousVialEffect>
	{
		public override void SafeAddRecipes()
		{
			Recipe recipe = CreateRecipe(1);
			recipe.AddIngredient(ItemID.Bottle, 1);
			recipe.AddIngredient(ItemID.RegenerationPotion, 45);
			recipe.AddIngredient(ItemID.NeonTetra, 35);
			recipe.AddIngredient(ItemID.DaybloomSeeds, 600);
			recipe.DisableDecraft();
			recipe.Register();
		}
	}
}
