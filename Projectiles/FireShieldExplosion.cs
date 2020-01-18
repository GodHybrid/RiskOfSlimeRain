using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace RiskOfSlimeRain.Projectiles
{
	public class FireShieldExplosion : InstantExplosion
	{
		public override void SetStaticDefaults()
		{
			//Main.projFrames[projectile.type] = 2;
		}

		public override void SetDefaults()
		{
			base.SetDefaults();
			projectile.Size = new Vector2(200);
		}

		public override void Kill(int timeLeft)
		{
			Main.PlaySound(SoundID.DD2_ExplosiveTrapExplode);
			for (int i = 0; i < 20; i++) //40
			{
				Dust dust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.Smoke, 0f, 0f, 100, default(Color), 2f);
				dust.velocity *= 2f; //3f
				if (Main.rand.NextBool(2))
				{
					dust.scale = 0.5f;
					dust.fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
				}
			}
			for (int i = 0; i < 35; i++) //70
			{
				Dust dust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.Fire, 0f, 0f, 100, default(Color), 3f);
				dust.noGravity = true;
				dust.velocity *= 4f; //5f
				dust.noLight = true;
				dust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.Fire, 0f, 0f, 100, default(Color), 2f);
				dust.velocity *= 2f;
				dust.noLight = true;
			}
			for (int i = 0; i < 2; i++) //3
			{
				float scaleFactor10 = 0.33f;
				if (i == 1)
				{
					scaleFactor10 = 0.66f;
				}
				if (i == 2)
				{
					scaleFactor10 = 1f;
				}
				Gore gore = Gore.NewGoreDirect(new Vector2(projectile.Center.X - 24f, projectile.Center.Y - 24f), default(Vector2), Main.rand.Next(61, 64), 1f);
				gore.velocity *= scaleFactor10;
				gore.velocity.X += 1f;
				gore.velocity.Y += 1f;
				gore = Gore.NewGoreDirect(new Vector2(projectile.Center.X - 24f, projectile.Center.Y - 24f), default(Vector2), Main.rand.Next(61, 64), 1f);
				gore.velocity *= scaleFactor10;
				gore.velocity.X += -1f;
				gore.velocity.Y += 1f;
				gore = Gore.NewGoreDirect(new Vector2(projectile.Center.X - 24f, projectile.Center.Y - 24f), default(Vector2), Main.rand.Next(61, 64), 1f);
				gore.velocity *= scaleFactor10;
				gore.velocity.X += 1f;
				gore.velocity.Y += -1f;
				gore = Gore.NewGoreDirect(new Vector2(projectile.Center.X - 24f, projectile.Center.Y - 24f), default(Vector2), Main.rand.Next(61, 64), 1f);
				gore.velocity *= scaleFactor10;
				gore.velocity.X += -1f;
				gore.velocity.Y += -1f;
			}
		}
	}
}
