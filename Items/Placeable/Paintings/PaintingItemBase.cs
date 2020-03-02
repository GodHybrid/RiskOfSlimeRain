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
		/// <summary>
		/// Name of the painting
		/// </summary>
		public abstract string PaintingName { get; }

		/// <summary>
		/// Name of the author displayed in the tooltip
		/// </summary>
		public abstract string PaintingAuthor { get; }

		public sealed override void SetStaticDefaults()
		{
			DisplayName.SetDefault(PaintingName);
			Tooltip.SetDefault($"'{PaintingAuthor}'");
		}

		public sealed override void SetDefaults()
		{
			item.value = Item.buyPrice(gold: 1);
			item.width = 32;
			item.height = 32;
			item.maxStack = 999;
			item.useTurn = true;
			item.autoReuse = true;
			item.useAnimation = 15;
			item.useTime = 10;
			item.useStyle = ItemUseStyleID.SwingThrow;
			item.consumable = true;
			item.rare = ItemRarityID.Blue;
			item.createTile = TileType;
		}
	}
}
