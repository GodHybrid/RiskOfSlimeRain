using Terraria.ModLoader;

namespace RiskOfSlimeRain.Tiles
{
	/// <summary>
	/// Simple ModTile class tied to an ModItem class, providing the item type
	/// </summary>
	public abstract class DroppableTile<T> : ModTile where T : ModItem
	{
		public int ItemType => ModContent.ItemType<T>();
	}
}
