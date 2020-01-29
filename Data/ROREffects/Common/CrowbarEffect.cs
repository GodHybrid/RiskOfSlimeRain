using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Data.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Projectiles;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Data.ROREffects.Common
{
	public class CrowbarEffect : RORCommonEffect, IModifyHit
	{
		const float initial = 0.2f;
		const float increase = 0.3f;
		const float hplimit = 0.8f;

		public override string Description => $"Deal {(initial + increase).ToPercent()} more damage to enemies above {hplimit.ToPercent()} HP";

		public override string FlavorText => "Crowbar/prybar/wrecking bar allows for both prying and smashing! \nCarbon steel, so it should last for a very long time, at least until the 3rd edition arrives";

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
			if (target.life >= target.lifeMax * hplimit)
			{
				Projectile.NewProjectile(target.Center, Vector2.Zero, ModContent.ProjectileType<CrowbarProj>(), 0, 0, Main.myPlayer, 0, target.whoAmI);
				damage += (int)(damage * (initial + increase * Stack));
			}
		}
	}
}
