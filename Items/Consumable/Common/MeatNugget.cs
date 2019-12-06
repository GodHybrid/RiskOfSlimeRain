using RiskOfSlimeRain.Effects.Common;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable.Common
{
	public class MeatNugget : RORConsumableItem<MeatNuggetEffect>
	{
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Gel, 9990);
			recipe.AddIngredient(ItemID.PinkGel, 400);
			recipe.AddRecipeGroup("RoR:EvilMaterial", 999);
			recipe.AddIngredient(ItemID.Bunny, 100);
			recipe.AddIngredient(ItemID.HealingPotion, 250);
			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}
