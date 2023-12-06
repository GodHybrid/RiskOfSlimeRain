using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Projectiles;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Core.ROREffects.Common
{
	public class CrowbarEffect : RORCommonEffect, IModifyHit
	{
		//const float Initial = 0.2f;
		//const float Increase = 0.3f;
		private float HealthLimit => ServerConfig.Instance.OriginalStats ? 0.8f : 0.85f;

		public override float Initial => 0.5f;

		public override float Increase => ServerConfig.Instance.OriginalStats ? 0.3f : 0.1f;

		public override LocalizedText Description => base.Description.WithFormatArgs(Initial.ToPercent(), HealthLimit.ToPercent());

		public override string UIInfo()
		{
			return $"Damage increase: {Formula().ToPercent()}";
		}

		public void ModifyHitNPC(Player player, Item item, NPC target, ref NPC.HitModifiers modifiers)
		{
			ModifyDamage(player, target, ref modifiers);
		}

		public void ModifyHitNPCWithProj(Player player, Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
		{
			ModifyDamage(player, target, ref modifiers);
		}

		void ModifyDamage(Player player, NPC target, ref NPC.HitModifiers modifiers)
		{
			if (target.life >= target.lifeMax * HealthLimit)
			{
				Projectile.NewProjectile(GetEntitySource(player), target.Center, Vector2.Zero, ModContent.ProjectileType<CrowbarProj>(), 0, 0, Main.myPlayer, 0, target.whoAmI);
				modifiers.SourceDamage += Formula();
			}
		}
	}
}
