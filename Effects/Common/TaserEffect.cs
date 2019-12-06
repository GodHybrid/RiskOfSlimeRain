using RiskOfSlimeRain.Buffs;
using RiskOfSlimeRain.Effects.Interfaces;
using System;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Effects.Common
{
	public class TaserEffect : ROREffect, IOnHit
	{
		const int initial = 5;
		const int increase = 5;

		public override string Description => "7% chance to snare enemies for 1.5 seconds";

		public override string FlavorText => "You say you can fix 'em?\nThese tasers are very very faulty";

		public override float Chance => 0.07f;

		public void OnHitNPC(Player player, Item item, NPC target, int damage, float knockback, bool crit)
		{
			AddBuff(target);
		}

		public void OnHitNPCWithProj(Player player, Projectile proj, NPC target, int damage, float knockback, bool crit)
		{
			AddBuff(target);
		}

		void AddBuff(NPC target)
		{
			target.AddBuff(ModContent.BuffType<TaserImmobility>(), initial + increase * Stack);
		}
	}
}
