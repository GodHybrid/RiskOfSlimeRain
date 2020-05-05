using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Projectiles;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Core.ROREffects.Common
{
	public class CrowbarEffect : RORCommonEffect, IModifyHit
	{
		//const float Initial = 0.2f;
		//const float Increase = 0.3f;
		private float HPlimit => ServerConfig.Instance.OriginalStats ? 0.8f : 0.85f;

		public override float Initial => 0.5f;

		public override float Increase => ServerConfig.Instance.OriginalStats ? 0.3f : 0.1f;

		public override string Description => $"Deal {Initial.ToPercent()} more damage to enemies above {HPlimit.ToPercent()} HP";

		public override string FlavorText => "Crowbar/prybar/wrecking bar allows for both prying and smashing! \nCarbon steel, so it should last for a very long time, at least until the 3rd edition arrives";

		public override string UIInfo()
		{
			return $"Damage increase: {Formula().ToPercent()}";
		}

		public void ModifyHitNPC(Player player, Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
		{
			ModifyDamage(target, ref damage);
		}

		public void ModifyHitNPCWithProj(Player player, Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			ModifyDamage(target, ref damage);
		}

		void ModifyDamage(NPC target, ref int damage)
		{
			if (target.life >= target.lifeMax * HPlimit)
			{
				Projectile.NewProjectile(target.Center, Vector2.Zero, ModContent.ProjectileType<CrowbarProj>(), 0, 0, Main.myPlayer, 0, target.whoAmI);
				damage += (int)(damage * Formula());
			}
		}
	}
}
