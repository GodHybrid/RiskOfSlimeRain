using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Placeable.Paintings
{
	/// <summary>
	/// Base class for all painting items
	/// </summary>
	public abstract class PaintingItemBase<T> : PlaceableItem<T> where T : ModTile
	{
		public sealed override void SetStaticDefaults()
		{
			// DisplayName.SetDefault(PaintingName);
			// Tooltip.SetDefault($"'{PaintingAuthor}'");
		}

		public sealed override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(TileType);
			Item.height = 32;
			Item.width = 32;
			Item.value = Item.buyPrice(gold: 1);
			Item.rare = ItemRarityID.Blue;
			Item.createTile = TileType;
		}
	}
}
