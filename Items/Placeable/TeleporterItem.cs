using RiskOfSlimeRain.Tiles.SubworldTiles;
using Terraria.ID;

namespace RiskOfSlimeRain.Items.Placeable
{
	//TODO remove/uncomment, debug item
	public class TeleporterItem : PlaceableItem<TeleporterTile>
	{
		public override string Texture => "RiskOfSlimeRain/Items/Placeable/GeyserItem";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Teleporter");
			Tooltip.SetDefault("");
		}

		public override void SetDefaults()
		{ 
			item.width = 34;
			item.height = 10;
			item.maxStack = 99;
			item.value = 0;
			item.rare = ItemRarityID.Orange;
			item.createTile = TileType;
			item.useTurn = true;
			item.autoReuse = true;
			item.useAnimation = 15;
			item.useTime = 10;
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.consumable = true;
		}
	}
}
