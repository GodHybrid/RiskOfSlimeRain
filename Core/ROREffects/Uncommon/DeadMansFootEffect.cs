using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Dusts;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Projectiles;
using System;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Core.ROREffects.Uncommon
{
	public class DeadMansFootEffect : RORUncommonEffect, IPostHurt, IPostUpdateEquips
	{
		public override float Initial => 4;

		public override float Increase => 1;

		public const float damage = 1.5f;
		public const int damageFlat = 30;
		public const float damageThreshold = 0.175f;
		public const float lowHealthThreshold = 0.15f;

		private float Ticks => Formula();

		public override LocalizedText Description => base.Description.WithFormatArgs(Initial, damage.ToPercent());

		public override string UIInfo()
		{
			return $"Debuff duration: {30 * (int)Formula()}";
		}

		public void PostHurt(Player player, Player.HurtInfo info)
		{
			if (Main.myPlayer == player.whoAmI && 
				(damage >= Math.Max(damageFlat, (int)(player.statLifeMax2 * damageThreshold)) || player.statLife <= (int)(player.statLifeMax2 * lowHealthThreshold))	)
			{
				int damageForProj = player.GetDamage();
				Projectile.NewProjectile(GetEntitySource(player), player.Center, Vector2.Zero, ModContent.ProjectileType<DeadMansFootMineProj>(), 0, 0, Main.myPlayer, damageForProj, Ticks);
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
