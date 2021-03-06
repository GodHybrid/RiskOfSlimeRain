﻿using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Projectiles;
using System;
using Terraria;

namespace RiskOfSlimeRain.Core.ROREffects.Common
{
	public class RustyKnifeEffect : RORCommonEffect, IOnHit
	{
		//const float Initial = 0.15f;
		private float Damage => ServerConfig.Instance.OriginalStats ? 0.35f : 0.3f;

		public override float Initial => ServerConfig.Instance.OriginalStats ? 0.15f : 0.1f;

		public override float Increase => ServerConfig.Instance.OriginalStats ? 0.15f : 0.05f;

		public override int MaxRecommendedStack => ServerConfig.Instance.OriginalStats ? 7 : 19;

		public override string Description => $"{Initial.ToPercent()} chance to cause bleeding. Bleeding deals {RustyKnifeProj.tickAmount}x{Damage.ToPercent()} damage";

		public override string FlavorText => "Murder weapon, case name ELIAS. Probably a lover's spat?\nThere is still dried blood on the knife, so mark it as biological.";

		public override string UIInfo()
		{
			return $"Chance: {Math.Min(Chance, 1f).ToPercent()}";
		}

		public override bool AlwaysProc => false;

		public override float Chance => Formula();

		public void OnHitNPC(Player player, Item item, NPC target, int damage, float knockback, bool crit)
		{
			SpawnProjectile(player, target);
		}

		public void OnHitNPCWithProj(Player player, Projectile proj, NPC target, int damage, float knockback, bool crit)
		{
			SpawnProjectile(player, target);
		}

		void SpawnProjectile(Player player, NPC target)
		{
			int damage = (int)(Damage * player.GetDamage());
			StickyProj.NewProjectile<RustyKnifeProj>(target, damage: damage);
		}
	}
}
