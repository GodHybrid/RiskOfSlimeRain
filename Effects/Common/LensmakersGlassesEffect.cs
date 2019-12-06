﻿using RiskOfSlimeRain.Effects.Interfaces;
using Terraria;

namespace RiskOfSlimeRain.Effects.Common
{
	public class LensmakersGlassesEffect : ROREffect, IModifyHit
	{
		const float increase = 0.07f;

		public override int MaxStack => 14;

		public override string Name => "Lens-Maker's Glasses";

		public override string Description => "Increases crit chance by 7%";

		public override string FlavorText => "Calibrated for high focal alignment\nShould allow for the precision you were asking for";

		public override float Chance => increase * Stack;

		public void ModifyHitNPC(Player player, Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
		{
			crit = true;
		}

		public void ModifyHitNPCWithProj(Player player, Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			crit = true;
		}
	}
}
