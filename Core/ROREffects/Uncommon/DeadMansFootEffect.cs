﻿using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Dusts;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Projectiles;
using System;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Core.ROREffects.Uncommon
{
	public class DeadMansFootEffect : RORUncommonEffect, IPostHurt, IPostUpdateEquips
	{
		public override float Initial => 4;

		public override float Increase => 1;

		public const float damage = 1.5f;
		public const int damageLow = 30;
		public const float damageHigh = 0.175f;

		private float Ticks => Formula();

		public override string Description => $"Drop a poison mine at low health or after taking significant damage for {Initial}x{damage.ToPercent()} damage";

		public override string FlavorText => "It looks like he was infested by some bug-like creatures, and exploded when I got close.\nI hope his death wasn't too painful; his family will know how he died.";

		public override string Name => "Dead Man's Foot";

		public override string UIInfo()
		{
			return $"Debuff duration: {30 * (int)Formula()}";
		}

		public void PostHurt(Player player, bool pvp, bool quiet, double damage, int hitDirection, bool crit)
		{
			if (Main.myPlayer == player.whoAmI && damage >= Math.Max(damageLow, (int)(player.statLifeMax2 * damageHigh)) || player.statLife <= (int)(player.statLifeMax2 * 0.15f))
			{
				Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<DeadMansFootMineProj>(), 0, 0, Main.myPlayer, player.GetDamage(), Ticks);
			}
		}

		public void PostUpdateEquips(Player player)
		{
			if (Config.HiddenVisuals(player)) return;

			if (Main.rand.NextBool(30))
			{
				Dust dust = Dust.NewDustDirect(player.Left, player.width, player.height >> 1, ModContent.DustType<ColorableDustAlphaFade>(), 0, 0, 0, Color.LightYellow * 0.78f, 1.25f);
				dust.customData = new InAndOutData(inSpeed: 30, outSpeed: 10, reduceScale: false);
				dust.velocity.X *= 0f;
				dust.velocity.Y = 0.3f;
			}
		}
	}
}
