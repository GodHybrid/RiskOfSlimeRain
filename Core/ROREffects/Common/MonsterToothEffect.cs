﻿using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Projectiles;
using Terraria;

namespace RiskOfSlimeRain.Core.ROREffects.Common
{
	public class MonsterToothEffect : RORCommonEffect, IOnHit
	{
		const int initial = 5;
		const int increase = 5;

		public override string Description => $"Killing an enemy will heal you for {initial + increase} health";

		public override string FlavorText => "Sometimes I felt like it helped me on hunts, ya know?\nLike... instincts";

		public void OnHitNPC(Player player, Item item, NPC target, int damage, float knockback, bool crit)
		{
			SpawnProjectile(target);
		}

		public void OnHitNPCWithProj(Player player, Projectile proj, NPC target, int damage, float knockback, bool crit)
		{
			SpawnProjectile(target);
		}

		void SpawnProjectile(NPC target)
		{
			if (target.life <= 0)
			{
				PlayerBonusProj.NewProjectile(target.Center, new Vector2(0f, -10f), onCreate: delegate (PlayerHealthProj proj)
				{
					proj.HealAmount = Stack * increase + initial;
				});
			}
		}
	}
}