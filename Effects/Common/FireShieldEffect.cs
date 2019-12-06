﻿using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Effects.Interfaces;
using RiskOfSlimeRain.Projectiles;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Effects.Common
{
	public class FireShieldEffect : ROREffect, IPostHurt
	{
		const int dmg = 200;
		const float kb = 20;

		public override string Description => "After being hit for 10% of your max health - explode, dealing 200 damage";

		public override string FlavorText => "The thing is only half-done, but it will do the job\nPLEASE handle with care!";

		public void PostHurt(Player player, bool pvp, bool quiet, double damage, int hitDirection, bool crit)
		{
			if (damage >= player.statLifeMax2 / 10)
			{
				Projectile.NewProjectile(player.position, Vector2.Zero, ModContent.ProjectileType<FireShieldExplosion>(), (dmg + dmg * Stack) * player.GetWeaponDamage(player.HeldItem), kb + Stack, Main.myPlayer);
			}
		}
	}
}
