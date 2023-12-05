using RiskOfSlimeRain.Core.ROREffects.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable.Common
{
	public class SproutingEgg : RORConsumableItem<SproutingEggEffect>
	{
		public override void SafeAddRecipes()
		{
			Recipe recipe = CreateRecipe(1);
			recipe.AddIngredient(ItemID.Vine, 100);
			recipe.AddIngredient(ItemID.Scorpion, 10);
			recipe.AddIngredient(ItemID.StrangeBrew, 60);
			recipe.AddIngredient(ItemID.Seed, 450);
			recipe.Register();
		}
	}
}
