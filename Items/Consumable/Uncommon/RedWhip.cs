using RiskOfSlimeRain.Core.ROREffects.Uncommon;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable.Uncommon
{
	public class RedWhip : RORConsumableItem<RedWhipEffect>
	{
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddRecipeGroup("RoR:FastBoots", 1);
			recipe.AddIngredient(ItemID.Bone, 355);
			recipe.AddIngredient(ItemID.FossilOre, 280);
			recipe.AddIngredient(ItemID.Rally, 3);

			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}
