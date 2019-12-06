using RiskOfSlimeRain.Buffs;
using RiskOfSlimeRain.Effects.Interfaces;
using System;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Effects.Common
{
	public class WarbannerEffect : ROREffect, IOnHit
	{
		public override string Description => "Chance to drop an empowering banner when killing an enemy";

		public override string FlavorText => "Very very valuable\nDon't drop it; it's worth more than you";

		public override float Chance => 0.00f;

		public void OnHitNPC(Player player, Item item, NPC target, int damage, float knockback, bool crit)
		{
			//AddBuff(target);
		}

		public void OnHitNPCWithProj(Player player, Projectile proj, NPC target, int damage, float knockback, bool crit)
		{
			//AddBuff(target);
		}

		void AddBuff(NPC target)
		{
			target.AddBuff(ModContent.BuffType<TaserImmobility>(), Stack);
		}
	}
}
