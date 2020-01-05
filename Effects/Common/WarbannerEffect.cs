﻿using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Data.Warbanners;
using RiskOfSlimeRain.Effects.Interfaces;
using Terraria;

namespace RiskOfSlimeRain.Effects.Common
{
	public class WarbannerEffect : ROREffect, IOnHit
	{
		const int initial = 4;
		const int increase = 1;

		public override string Description => "Chance to drop an empowering banner when killing an enemy";

		public override string FlavorText => "Very very valuable\nDon't drop it; it's worth more than you";

		public override bool AlwaysProc => true; //false

		public override float Chance => 1f;

		public void OnHitNPC(Player player, Item item, NPC target, int damage, float knockback, bool crit)
		{
			if (target.life <= 0) PassStatsIntoWarbanner(player);
		}

		public void OnHitNPCWithProj(Player player, Projectile proj, NPC target, int damage, float knockback, bool crit)
		{
			if (target.life <= 0) PassStatsIntoWarbanner(player);
		}

		void PassStatsIntoWarbanner(Player player)
		{
			WarbannerManager.TryAddWarbanner((initial + increase * Stack) * 16, player.Center + new Vector2(0f, -32f));
		}
	}
}
