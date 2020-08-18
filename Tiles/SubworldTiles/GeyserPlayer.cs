using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace RiskOfSlimeRain.Tiles.SubworldTiles
{
	public class GeyserPlayer : ModPlayer
	{
		public bool inGeyser = false;
		public bool lastInGeyser = false;

		public override void PostUpdate()
		{
			if (Main.netMode != NetmodeID.Server && player.whoAmI == Main.myPlayer && !player.controlDown && !(player.mount != null && player.mount.CanFly))
			{
				lastInGeyser = inGeyser;

				Point pos = new Vector2(player.Center.X, player.Bottom.Y - 8f).ToTileCoordinates();
				inGeyser = GeyserTile.GetGeyserHitbox(pos.X, pos.Y, out _);

				if (!lastInGeyser && inGeyser)
				{
					GeyserTile.Jump(player);
				}
			}
		}
	}
}
