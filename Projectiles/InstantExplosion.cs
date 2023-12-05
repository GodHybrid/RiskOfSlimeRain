using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
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
			Projectile.Size = new Vector2(16);
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.alpha = 255;
			Projectile.timeLeft = 3;

			Projectile.usesIDStaticNPCImmunity = true;
			Projectile.idStaticNPCHitCooldown = 10;
		}

		public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
		{
			//To apply proper knockback based on what side the explosion is
			modifiers.HitDirectionOverride = (target.Center.X > Projectile.Center.X).ToDirectionInt();
		}

		public override void AI()
		{
			Projectile.Damage(); //To apply damage
			Projectile.Kill(); //Do disappear right after it
		}
	}
}
