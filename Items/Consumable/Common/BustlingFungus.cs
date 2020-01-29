using RiskOfSlimeRain.Data.ROREffects.Common;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable.Common
{
	public class BustlingFungus : RORConsumableItem<BustlingFungusEffect>
	{
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.GlowingMushroom, 500);
			recipe.AddIngredient(ItemID.RestorationPotion, 200);
			recipe.AddIngredient(ItemID.JungleSpores, 250);
			recipe.AddIngredient(ItemID.TealMushroom, 50);
			recipe.AddIngredient(ItemID.GreenMushroom, 50);
			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}
