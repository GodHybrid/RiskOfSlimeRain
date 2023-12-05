using RiskOfSlimeRain.Core.ROREffects.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable.Common
{
	public class HermitsScarf : RORConsumableItem<HermitsScarfEffect>
	{
		public override void SafeAddRecipes()
		{
			Recipe recipe = CreateRecipe(1);
			recipe.AddIngredient(ItemID.Silk, 490);
			recipe.AddIngredient(ItemID.Feather, 150);
			recipe.AddIngredient(ItemID.Gi, 5);
			recipe.AddIngredient(ItemID.TrapsightPotion, 20);
			recipe.Register();
		}
	}
}
