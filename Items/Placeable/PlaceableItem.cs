using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Placeable
{
	/// <summary>
	/// Simple ModItem class tied to a ModTile class, providing the tile type
	/// </summary>
	public abstract class PlaceableItem<T> : ModItem where T : ModTile
	{
		public int TileType => ModContent.TileType<T>();
	}
}
