using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Projectiles;
using System;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Core.ROREffects.Common
{
	public class FireShieldEffect : RORCommonEffect, IPostHurt
	{
		private float Damage => ServerConfig.Instance.OriginalStats ? 4f : 6f;

		private float KB => ServerConfig.Instance.OriginalStats ? 20 : 15;

		private float HPlimit => ServerConfig.Instance.OriginalStats ? 0.1f : 0.2f;

		public override LocalizedText Description => base.Description.WithFormatArgs(HPlimit.ToPercent(), Damage.ToPercent());

		public override string UIInfo()
		{
			return UIInfoText.Format((Damage * Stack).ToPercent(), (KB + Stack).ToPercent());
		}

		public void PostHurt(Player player, Player.HurtInfo info)
		{
			if (Main.myPlayer == player.whoAmI && info.Damage >= player.statLifeMax2 * HPlimit)
			{
				Projectile.NewProjectile(GetEntitySource(player), player.position, Vector2.Zero, ModContent.ProjectileType<FireShieldExplosion>(), (int)(Damage * Stack * player.GetDamage()), KB + Stack, Main.myPlayer);
			}
		}
	}
}
