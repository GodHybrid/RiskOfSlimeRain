using RiskOfSlimeRain.Core.ROREffects.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable.Common
{
	public class BustlingFungus : RORConsumableItem<BustlingFungusEffect>
	{
		public override void SafeAddRecipes()
		{
			Recipe recipe = CreateRecipe(1);
			recipe.AddIngredient(ItemID.GlowingMushroom, 300);
			recipe.AddIngredient(ItemID.JungleSpores, 250);
			recipe.AddIngredient(ItemID.TealMushroom, 10);
			recipe.AddIngredient(ItemID.GreenMushroom, 10);
			recipe.Register();
		}
	}
}
