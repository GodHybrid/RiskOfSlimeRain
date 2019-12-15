using RiskOfSlimeRain.Effects.Interfaces;
using RiskOfSlimeRain.Helpers;
using System;
using Terraria;

namespace RiskOfSlimeRain.Effects.Common
{
	public class SproutingEggEffect : ROREffect, IUpdateLifeRegen, IPostHurt, IOnHit
	{
		const float increase = 2.4f;
		const int timerMax = 420;
		int timer = timerMax;

		public override string Description => $"Permanently increases health regeneration by {increase} health per second when out of combat for {timerMax/60} seconds";

		public override string FlavorText => "This egg seems to be somewhere between hatching and dying\nI can't bring it to myself to cook it alive";

		public void OnHitNPC(Player player, Item item, NPC target, int damage, float knockback, bool crit)
		{
			ResetTimer();
		}

		public void OnHitNPCWithProj(Player player, Projectile proj, NPC target, int damage, float knockback, bool crit)
		{
			ResetTimer();
		}

		public void PostHurt(Player player, bool pvp, bool quiet, double damage, int hitDirection, bool crit)
		{
			ResetTimer();
		}

		public void UpdateLifeRegen(Player player)
		{
			if (timer > 0) timer--;
			//the number will be halved in redcode, hence the 2
			if (timer == 0) player.lifeRegen += (int)Math.Round(Stack * 2 * increase);
		}

		void ResetTimer()
		{
			timer = timerMax;
		}
	}
}
