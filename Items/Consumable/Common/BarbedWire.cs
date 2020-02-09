using RiskOfSlimeRain.Core.ROREffects.Common;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable.Common
{
	public class BarbedWire : RORConsumableItem<BarbedWireEffect>
	{
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			//RORPlayer p = ModContent.GetInstance<RORPlayer>();
			//int present = ROREffectManager.GetEffectOfType<BarbedWireEffect>(p).Stack;
			recipe.AddIngredient(RecipeGroupID.IronBar, 60);
			recipe.AddIngredient(ItemID.ThornChakram, 3);
			recipe.AddIngredient(ItemID.SharkToothNecklace, 5);
			recipe.AddIngredient(ItemID.Wire, 210);

			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}
