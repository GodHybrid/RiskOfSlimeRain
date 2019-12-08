using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Effects.Interfaces;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Projectiles
{
	public class StickyBombExplosion : ModProjectile, IExcludeOnHit
	{
		public override void SetDefaults()
		{
			projectile.width = 16;
			projectile.height = 16;
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

		public override void Kill(int timeLeft)
		{
			Main.PlaySound(SoundID.DD2_ExplosiveTrapExplode);
			for (int i = 0; i < 2; i++)
			{
				Dust dust = Dust.NewDustDirect(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, DustID.Smoke, 0f, 0f, 100, default(Color), 2f);
				dust.velocity *= 2f; //3f
				if (Main.rand.NextBool(2))
				{
					dust.scale = 0.5f;
					dust.fadeIn = 1f + Main.rand.Next(10) * 0.1f;
				}
			}
			for (int i = 0; i < 5; i++)
			{
				Dust dust = Dust.NewDustDirect(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, DustID.Fire, 0f, 0f, 100, default(Color), 3f);
				dust.noGravity = true;
				dust.velocity *= 4f; //5f
				dust = Dust.NewDustDirect(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, DustID.Fire, 0f, 0f, 100, default(Color), 2f);
				dust.velocity *= 2f;
			}
			//for (int i = 0; i < 2; i++) 
			//{
			//	float scaleFactor10 = 0.33f;
			//	if (i == 1)
			//	{
			//		scaleFactor10 = 0.66f;
			//	}
			//	if (i == 2)
			//	{
			//		scaleFactor10 = 1f;
			//	}
			//	Gore gore = Main.gore[Gore.NewGore(new Vector2(projectile.position.X + (float)(projectile.width / 2) - 24f, projectile.position.Y + (float)(projectile.height / 2) - 24f), default(Vector2), Main.rand.Next(61, 64), 1f)];
			//	gore.velocity *= scaleFactor10;
			//	gore.velocity.X += 1f;
			//	gore.velocity.Y += 1f;
			//	gore = Main.gore[Gore.NewGore(new Vector2(projectile.position.X + (float)(projectile.width / 2) - 24f, projectile.position.Y + (float)(projectile.height / 2) - 24f), default(Vector2), Main.rand.Next(61, 64), 1f)];
			//	gore.velocity *= scaleFactor10;
			//	gore.velocity.X += -1f;
			//	gore.velocity.Y += 1f;
			//	gore = Main.gore[Gore.NewGore(new Vector2(projectile.position.X + (float)(projectile.width / 2) - 24f, projectile.position.Y + (float)(projectile.height / 2) - 24f), default(Vector2), Main.rand.Next(61, 64), 1f)];
			//	gore.velocity *= scaleFactor10;
			//	gore.velocity.X += 1f;
			//	gore.velocity.Y += -1f;
			//	gore = Main.gore[Gore.NewGore(new Vector2(projectile.position.X + (float)(projectile.width / 2) - 24f, projectile.position.Y + (float)(projectile.height / 2) - 24f), default(Vector2), Main.rand.Next(61, 64), 1f)];
			//	gore.velocity *= scaleFactor10;
			//	gore.velocity.X += -1f;
			//	gore.velocity.Y += -1f;
			//}
		}
	}
}
