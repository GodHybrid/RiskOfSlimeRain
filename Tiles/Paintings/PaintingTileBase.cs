using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace RiskOfSlimeRain.Tiles.Paintings
{
	/// <summary>
	/// Base class for all painting tiles
	/// </summary>
	public abstract class PaintingTileBase : ModTile
	{
		//TODO style classes based on dimension
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
			//TileObjectData.newTile.StyleHorizontal = true;
			//TileObjectData.newTile.StyleWrapLimit = 36;
			TileObjectData.addTile(Type);
			DustType = 7;
			TileID.Sets.DisableSmartCursor[Type] = true;
		}
	}
}
