using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace RiskOfSlimeRain.Projectiles
{
	public class StickyBombExplosion : InstantExplosion
	{
		public override void SetDefaults()
		{
			base.SetDefaults();
		}

		public override void OnKill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode);
			for (int i = 0; i < 2; i++)
			{
				Dust dust = Dust.NewDustDirect(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100, default(Color), 2f);
				dust.velocity *= 2f; //3f
				if (Main.rand.NextBool(2))
				{
					dust.scale = 0.5f;
					dust.fadeIn = 1f + Main.rand.Next(10) * 0.1f;
				}
			}
			for (int i = 0; i < 5; i++)
			{
				Dust dust = Dust.NewDustDirect(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default(Color), 3f);
				dust.noGravity = true;
				dust.velocity *= 4f; //5f
				dust.noLight = true;
				dust = Dust.NewDustDirect(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100, default(Color), 2f);
				dust.velocity *= 2f;
				dust.noLight = true;
			}
			//for (int i = 0; i < 2; i++) //3
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
			//	Gore gore = Gore.NewGoreDirect(new Vector2(projectile.position.X + (float)(projectile.width / 2) - 24f, projectile.position.Y + (float)(projectile.height / 2) - 24f), default(Vector2), Main.rand.Next(61, 64), 1f);
			//	gore.velocity *= scaleFactor10;
			//	gore.velocity.X += 1f;
			//	gore.velocity.Y += 1f;
			//	gore = Gore.NewGoreDirect(new Vector2(projectile.position.X + (float)(projectile.width / 2) - 24f, projectile.position.Y + (float)(projectile.height / 2) - 24f), default(Vector2), Main.rand.Next(61, 64), 1f);
			//	gore.velocity *= scaleFactor10;
			//	gore.velocity.X += -1f;
			//	gore.velocity.Y += 1f;
			//	gore = Gore.NewGoreDirect(new Vector2(projectile.position.X + (float)(projectile.width / 2) - 24f, projectile.position.Y + (float)(projectile.height / 2) - 24f), default(Vector2), Main.rand.Next(61, 64), 1f);
			//	gore.velocity *= scaleFactor10;
			//	gore.velocity.X += 1f;
			//	gore.velocity.Y += -1f;
			//	gore = Gore.NewGoreDirect(new Vector2(projectile.position.X + (float)(projectile.width / 2) - 24f, projectile.position.Y + (float)(projectile.height / 2) - 24f), default(Vector2), Main.rand.Next(61, 64), 1f);
			//	gore.velocity *= scaleFactor10;
			//	gore.velocity.X += -1f;
			//	gore.velocity.Y += -1f;
			//}
		}
	}
}
