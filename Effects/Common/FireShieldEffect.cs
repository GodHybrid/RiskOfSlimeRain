using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Effects.Interfaces;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Projectiles;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Effects.Common
{
	public class FireShieldEffect : RORCommonEffect, IPostHurt
	{
		const int dmg = 200;
		const float kb = 20;
		const float hplimit = 0.1f;

		public override string Description => $"After being hit for {hplimit.ToPercent()} of your max health - explode, dealing {dmg} damage";

		public override string FlavorText => "The thing is only half-done, but it will do the job\nPLEASE handle with care!";

		public void PostHurt(Player player, bool pvp, bool quiet, double damage, int hitDirection, bool crit)
		{
			if (damage >= player.statLifeMax2 * hplimit)
			{
				Projectile.NewProjectile(player.position, Vector2.Zero, ModContent.ProjectileType<FireShieldExplosion>(), (dmg + dmg * Stack) * player.GetDamage(), kb + Stack, Main.myPlayer);
			}
		}
	}
}
