using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Core.Subworlds
{
	/*
	public class DebugItem : ModItem
	{
		public override string Texture => "Terraria/Images/Item_" + ItemID.ReaverShark;

		public override void SetDefaults()
		{
			Item.maxStack = 1;
			Item.width = 34;
			Item.height = 38;
			Item.rare = 5;
			Item.useStyle = 4;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.UseSound = SoundID.Item1;
		}

		private void P(object s) => Main.NewText(s.ToString());

		public override bool? UseItem(Player player)
		{
			Tile tile = Framing.GetTileSafely(Player.tileTargetX, Player.tileTargetY);
			P($"T: {tile.TileType}, Fx: {tile.TileFrameX}, Fy: {tile.TileFrameY}");
			P($"W: {tile.WallType}, Wc: {tile.WallColor}");
			P($"S: {tile.Slope}, LT: {tile.LiquidType}, L%: {tile.LiquidAmount}");
			P($"C: {tile.TileColor}");
			P($"A: {tile.HasTile}, IA: {tile.IsActuated}, NA: {tile.HasUnactuatedTile}");
			return true;
		}
	}*/
}
