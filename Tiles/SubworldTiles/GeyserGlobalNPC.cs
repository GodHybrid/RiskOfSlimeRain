using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Tiles.SubworldTiles
{
	public class GeyserGlobalNPC : GlobalNPC
	{
		public override bool InstancePerEntity => true;

		public bool inGeyser = false;
		public bool lastInGeyser = false;

		public override void PostAI(NPC npc)
		{
			if (Main.netMode != NetmodeID.MultiplayerClient && !npc.noGravity && !npc.boss)
			{
				lastInGeyser = inGeyser;

				Point pos = new Vector2(npc.Center.X, npc.Bottom.Y - 8f).ToTileCoordinates();
				inGeyser = GeyserTile.GetGeyserHitbox(pos.X, pos.Y, out _);

				if (!lastInGeyser && inGeyser)
				{
					GeyserTile.Jump(npc);
					npc.netUpdate = true;
				}
			}
		}
	}
}
