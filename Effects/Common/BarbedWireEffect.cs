using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Effects.Interfaces;
using Terraria;

namespace RiskOfSlimeRain.Effects.Common
{
	public class BarbedWireEffect : ROREffect, IPostUpdateEquips
	{
		int wireTimer = 0;
		const int wireTimerMax = 60;
		const int wireRadius = 80;
		const float initial = 0.33f;
		const float increase = 0.17f;

		public override string Description => "Touching enemies deals 50% of your current damage every second";

		public override string FlavorText => "Disclaimer: I, or my company, am not responsible for any bodily harm delivered to...";

		public void PostUpdateEquips(Player player)
		{
			if (++wireTimer % wireTimerMax == 0)
			{
				for (int m = 0; m < Main.maxNPCs; m++)
				{
					NPC enemy = Main.npc[m];
					if (enemy.CanBeChasedBy() && Vector2.Distance(player.Center, enemy.Center) <= wireRadius * Stack)
					{
						enemy.StrikeNPC((int)((initial + increase * Stack) * player.GetWeaponDamage(player.HeldItem)), 0f, 0, false);
					}
				}
				wireTimer = 0;
			}
		}
	}
}
