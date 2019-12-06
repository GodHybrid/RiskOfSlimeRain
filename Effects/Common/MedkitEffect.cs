using RiskOfSlimeRain.Effects.Interfaces;
using System;
using Terraria;

namespace RiskOfSlimeRain.Effects.Common
{
	public class MedkitEffect : ROREffect, IPostUpdateEquips, IPostHurt
	{
		const int amount = 10;
		int timer = -1;

		public override string Description => "Heal for 10 health 1.1 seconds after receiving damage";

		public override string FlavorText => "Each Medkit should contain bandages, sterile dressings, soap,\nantiseptics, saline, gloves, scissors, aspirin, codeine, and an Epipen";

		public void PostUpdateEquips(Player player)
		{
			if (timer >= 0)
			{
				timer++;
				if (timer >= 66)
				{
					player.HealEffect(Stack * amount);
					player.statLife += Math.Min(Stack * amount, player.statLifeMax2 - player.statLife);
					timer = -1;
				}
			}
		}

		public void PostHurt(Player player, bool pvp, bool quiet, double damage, int hitDirection, bool crit)
		{
			timer = 0;
		}
	}
}
