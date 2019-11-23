using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Placeable
{
	public class UncoveredChest : ModItem
	{
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Hard to find, impossible to destroy.");
		}

		public override void SetDefaults()
		{
			item.width = 39;
			item.height = 33;
			item.maxStack = 2;
			item.useTurn = true;
			item.autoReuse = true;
			item.useAnimation = 15;
			item.useTime = 10;
			item.useStyle = 1;
			item.consumable = true;
			item.value = 50000000;
			item.createTile = mod.TileType("UncoveredChest");
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.LunarBar, 50);
			recipe.AddIngredient(mod.ItemType("SalvagedChest"), 2);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}