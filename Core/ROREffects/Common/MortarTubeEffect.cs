﻿using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Projectiles;
using Terraria;
using Terraria.ModLoader;
using Terraria.Localization;

namespace RiskOfSlimeRain.Core.ROREffects.Common
{
	public class MortarTubeEffect : RORCommonEffect, IOnHit
	{
		//const float Increase = 1.7f;

		public override float Initial => ServerConfig.Instance.OriginalStats ? 1.7f : 1.2f;

		public override float Increase => ServerConfig.Instance.OriginalStats ? 1.7f : 1.2f;

		public override LocalizedText Description => base.Description.WithFormatArgs(Chance.ToPercent(), Initial.ToPercent());

		public override string UIInfo()
		{
			return UIInfoText.Format(Formula().ToPercent());
		}

		public override bool AlwaysProc => false;

		public override float Chance => 0.09f;

		public void OnHitNPCWithItem(Player player, Item item, NPC target, NPC.HitInfo hit, int damageDone)
		{
			SpawnProjectile(player);
		}

		public void OnHitNPCWithProj(Player player, Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
		{
			SpawnProjectile(player);
		}

		void SpawnProjectile(Player player)
		{
			int damage = (int)(Formula() * player.GetDamage());
			Projectile.NewProjectile(GetEntitySource(player), player.Center - new Vector2(0, player.height / 2), new Vector2(5 * player.direction, -5), ModContent.ProjectileType<MortarTubeRocket>(), 0, 0, Main.myPlayer, damage);
		}
	}
}
