using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Effects.Interfaces;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Projectiles
{
	/// <summary>
	/// Basic explosion that will spawn and instantly damage things, then despawn. No visual effects by default
	/// </summary>
	public abstract class InstantExplosion : ModProjectile, IExcludeOnHit
	{
		public override string Texture => "RiskOfSlimeRain/Empty";

		public override void SetDefaults()
		{
			projectile.Size = new Vector2(16);
			projectile.friendly = true;
			projectile.hostile = false;
			projectile.penetrate = -1;
			projectile.tileCollide = false;
			projectile.alpha = 255;
			projectile.timeLeft = 3;

			projectile.usesIDStaticNPCImmunity = true;
			projectile.idStaticNPCHitCooldown = 10;
		}

		public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			//to apply proper knockback based on on what side the bomb is stuck on (left or right)
			hitDirection = (target.Center.X > projectile.Center.X).ToDirectionInt();
		}

		public override void AI()
		{
			projectile.Damage(); //to apply damage
			projectile.Kill(); //do disappear right after it
		}
	}
}
