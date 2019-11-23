using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Placeable
{
	public class SalvagedChest : ModItem
	{
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("It fell from such a height, and yet it's whole.");
		}

		public override void SetDefaults()
		{
			item.width = 26;
			item.height = 22;
			item.maxStack = 5;
			item.useTurn = true;
			item.autoReuse = true;
			item.useAnimation = 15;
			item.useTime = 10;
			item.useStyle = 1;
			item.consumable = true;
			item.value = 500000;
			item.createTile = mod.TileType("SalvagedChest");
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddRecipeGroup("RoR:Tier3HMBar", 90);
			recipe.AddRecipeGroup("RoR:AnyChest", 1);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}