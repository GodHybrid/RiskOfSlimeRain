using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Placeable
{
	public class EchPainting : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ech Painting");
			Tooltip.SetDefault("'Groalt W. Fai'");
		}

		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 20;
			item.maxStack = 999;
			item.useTurn = true;
			item.autoReuse = true;
			item.useAnimation = 15;
			item.useTime = 10;
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.consumable = true;
			item.rare = ItemRarityID.Blue;
			item.createTile = mod.TileType("EchPaintingTile");
		}
	}
}
