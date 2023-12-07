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
	public class HermitsScarfEffect : RORCommonEffect, IFreeDodge
	{
		//const float Initial = 0.05f;
		//const float Increase = 0.05f;

		public override float Initial => ServerConfig.Instance.OriginalStats ? 0.1f : 0.04f;

		public override float Increase => ServerConfig.Instance.OriginalStats ? 0.05f : 0.02f;

		public override int MaxRecommendedStack => ServerConfig.Instance.OriginalStats ? 6 : 9;

		public override bool EnforceMaxStack => true;

		public override LocalizedText Description => base.Description.WithFormatArgs(Initial.ToPercent());

		public override string UIInfo()
		{
			return UIInfoText.Format(Math.Min(Chance, 1f).ToPercent());
		}

		public override bool AlwaysProc => false;

		public override float Chance => Formula();

		public bool FreeDodge(Player player, Player.HurtInfo info)
		{
			player.immune = true;
			player.immuneTime = 60;
			if (player.longInvince)
			{
				player.immuneTime += 30;
			}
			for (int i = 0; i < player.hurtCooldowns.Length; i++)
			{
				player.hurtCooldowns[i] = player.immuneTime;
			}
			Projectile.NewProjectile(GetEntitySource(player), player.Center, new Vector2(0, -0.3f), ModContent.ProjectileType<HermitsScarfProj>(), 0, 0, Main.myPlayer);

			return true;
		}
	}
}
