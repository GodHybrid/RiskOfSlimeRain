using RiskOfSlimeRain.Core.ROREffects.Common;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable.Common
{
	public class Headstompers : RORConsumableItem<HeadstompersEffect>
	{
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.BootStatue, 5);
			recipe.AddIngredient(ItemID.NinjaPants, 5);
			recipe.AddIngredient(ItemID.Spike, 80);
			recipe.AddIngredient(ItemID.Leather, 55);

			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}
