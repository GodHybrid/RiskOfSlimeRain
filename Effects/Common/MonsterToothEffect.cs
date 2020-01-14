using RiskOfSlimeRain.Effects.Interfaces;
using RiskOfSlimeRain.Helpers;
using Terraria;

namespace RiskOfSlimeRain.Effects.Common
{
	public class MonsterToothEffect : RORCommonEffect, IOnHit
	{
		const int initial = 5;
		const int increase = 5;

		public override string Description => $"Killing an enemy will heal you for {initial + increase} health";

		public override string FlavorText => "Sometimes I felt like it helped me on hunts, ya know?\nLike... instincts";

		public void OnHitNPC(Player player, Item item, NPC target, int damage, float knockback, bool crit)
		{
			Heal(player, target);
		}

		public void OnHitNPCWithProj(Player player, Projectile proj, NPC target, int damage, float knockback, bool crit)
		{
			Heal(player, target);
		}

		void Heal(Player player, NPC target)
		{
			if (target.life <= 0)
			{
				int heal = Stack * increase + initial;
				player.HealMe(heal);
			}
		}
	}
}
