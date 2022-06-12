using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Projectiles;
using System;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Core.ROREffects.Common
{
	public class FireShieldEffect : RORCommonEffect, IPostHurt
	{
		private float Damage => ServerConfig.Instance.OriginalStats ? 4f : 6f;

		private float KB => ServerConfig.Instance.OriginalStats ? 20 : 15;

		private float HPlimit => ServerConfig.Instance.OriginalStats ? 0.1f : 0.2f;

		public override string Description => $"After being hit for {HPlimit.ToPercent()} of your max health - explode, dealing {Damage.ToPercent()} damage and knocking back enemies.";

		public override string FlavorText => "The thing is only half-done, but it will do the job\nPLEASE handle with care!";

		public override string UIInfo()
		{
			return $"Damage: {(Damage * Stack).ToPercent()}\nKnockback: {(KB + Stack).ToPercent()}";
		}

		public void PostHurt(Player player, bool pvp, bool quiet, double damage, int hitDirection, bool crit)
		{
			if (Main.myPlayer == player.whoAmI && damage >= player.statLifeMax2 * HPlimit)
			{
				Projectile.NewProjectile(player.position, Vector2.Zero, ModContent.ProjectileType<FireShieldExplosion>(), (int)(Damage * Stack * player.GetDamage()), KB + Stack, Main.myPlayer);
			}
		}
	}
}
