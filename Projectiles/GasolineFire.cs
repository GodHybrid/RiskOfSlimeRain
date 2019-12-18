using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Effects.Interfaces;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Projectiles
{
	public class GasolineFire : ModProjectile, IExcludeOnHit
	{
		public Rectangle newHitbox = default(Rectangle);

		public override string Texture => "RiskOfSlimeRain/Empty";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Gasoline fire");
		}

		public override void SetDefaults()
		{
			projectile.width = 8;
			projectile.height = 8;
			//projectile.penetrate = 3;
			projectile.penetrate = -1;
			//projectile.aiStyle = 14;
			projectile.friendly = true;
			//projectile.timeLeft = 360;
			projectile.timeLeft = 300;
			projectile.ranged = true;
			projectile.noEnchantments = true;

			projectile.usesIDStaticNPCImmunity = true;
			projectile.idStaticNPCHitCooldown = 30;
		}

		public override void ModifyDamageHitbox(ref Rectangle hitbox)
		{
			hitbox = newHitbox;
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			projectile.velocity.Y = 0f;
			return false;
		}

		public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
		{
			fallThrough = false;
			return base.TileCollideStyle(ref width, ref height, ref fallThrough);
		}

		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			target.AddBuff(BuffID.OnFire, 60);
		}

		public override void AI()
		{
			Hitbox();
			Movement();
			Visuals();
		}

		private void Hitbox()
		{
			newHitbox = projectile.Hitbox;
			newHitbox.Inflate(8, 2);
		}

		private void Movement()
		{
			projectile.ai[0] += 1f;

			if (projectile.ai[0] > 5f)
			{
				projectile.ai[0] = 5f;
				//stop spreading
				projectile.velocity.X *= 0.9f;
				if (projectile.velocity.X > -0.01f && projectile.velocity.X < 0.01f)
				{
					projectile.velocity.X = 0f;
					projectile.netUpdate = true;
				}
				//fall down
				projectile.velocity.Y = projectile.velocity.Y + 0.2f;
			}

			//terminal velocity cap
			if (projectile.velocity.Y > 16f)
			{
				projectile.velocity.Y = 16f;
			}

			//die if wet
			if (projectile.wet)
			{
				projectile.Kill();
			}
		}

		private void Visuals()
		{
			if (Main.rand.NextFloat() < 0.6f) return;
			//fire going up
			Dust dust = Dust.NewDustDirect(newHitbox.TopLeft(), newHitbox.Width, newHitbox.Height, DustID.Fire, 0f, 0f, 100);
			dust.position.X -= 2f;
			dust.position.Y += 2f;
			dust.scale += Main.rand.NextFloat(0.5f);
			dust.noGravity = true;
			dust.velocity.Y -= 2f;

			//static fire
			if (Main.rand.NextBool(2))
			{
				dust = Dust.NewDustDirect(newHitbox.TopLeft(), newHitbox.Width, newHitbox.Height, DustID.Fire, 0f, 0f, 100);
				dust.position.X -= 2f;
				dust.position.Y += 2f;
				dust.scale += 0.3f + Main.rand.NextFloat(0.5f);
				dust.noGravity = true;
				dust.velocity *= 0.1f;
			}
		}
	}
}
