using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Effects.Interfaces;
using System;
using Terraria;

namespace RiskOfSlimeRain.Effects.Common
{
	public class BustlingFungusEffect : ROREffect, IPostUpdateEquips
	{
		int noMoveTimer = 0;
		const int fungalRadius = 0;
		const int noMoveTimerMax = 120;

		public override string Description => "Grants \"Fungal Defense Mechanism\"\nStand still for 2 seconds to activate the buff\nHeals for 4.5% of your max HP every second";

		public override string FlavorText => "The strongest biological healing agent...\n...is a mushroom";

		public void PostUpdateEquips(Player player)
		{
			int totalFungusHeal = (int)(player.statLifeMax2 * 0.045f * Stack);
			//TODO rewrite the logic
			if (Equals(player.velocity, Vector2.Zero) && player.itemAnimation <= 0/*PlayerSolidTileCollision(player)*/)
			{
				noMoveTimer++;
				if (Main.myPlayer == player.whoAmI && noMoveTimer > noMoveTimerMax && noMoveTimer % noMoveTimerMax == 0)
				{
					foreach (NPC n in Main.npc)
					{
						if (n.active && n.townNPC && Vector2.Distance(player.position, n.position) < fungalRadius)
						{
							n.HealEffect((int)(totalFungusHeal), true);
							n.life += Math.Min(totalFungusHeal, n.lifeMax - n.life);
						}
					}
					if (Main.player.Length > 1)
					{
						foreach (Player n in Main.player)
						{
							if (n.active && Vector2.Distance(player.position, n.position) < fungalRadius)
							{
								player.HealEffect((int)(totalFungusHeal), true);
								player.statLife += (int)(totalFungusHeal);
							}
						}
					}
				}
			}
			else
			{
				noMoveTimer = 0;
			}
		}
	}
}
