using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace RiskOfSlimeRain.Projectiles
{
	public class FireShieldExplosion : InstantExplosion
	{
		public override void SetDefaults()
		{
			base.SetDefaults();
			Projectile.Size = new Vector2(200);
		}

		public override void OnKill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode);
			Vector2 randomPos = Projectile.Size / 2;
			for (int i = 0; i < 20; i++) //40
			{
				Vector2 randomDirection = randomPos.RotatedByRandom(Math.PI * 2) * Main.rand.NextFloat();
				Vector2 velocity = Vector2.Normalize(randomDirection);
				Dust dust = Dust.NewDustPerfect(Projectile.Center + randomDirection, DustID.Smoke, velocity, 100, default(Color), 2f);
				dust.velocity *= 2f; //3f
				if (Main.rand.NextBool(2))
				{
					dust.scale = 0.5f;
					dust.fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
				}
			}
			for (int i = 0; i < 35; i++) //70
			{
				Vector2 randomDirection = randomPos.RotatedByRandom(Math.PI * 2) * Main.rand.NextFloat();
				Vector2 velocity = Vector2.Normalize(randomDirection);
				Dust dust = Dust.NewDustPerfect(Projectile.Center + randomDirection, DustID.Torch, velocity, 100, default(Color), 3f);
				dust.noGravity = true;
				dust.velocity *= 4f; //5f
				dust.noLight = true;
				dust = Dust.NewDustPerfect(Projectile.Center + randomDirection, DustID.Torch, velocity, 100, default(Color), 2f);
				dust.velocity *= 2f;
				dust.noLight = true;
			}
			for (int i = 0; i < 2; i++) //3
			{
				float scaleFactor = 0.33f;
				if (i == 1)
				{
					scaleFactor = 0.66f;
				}
				if (i == 2)
				{
					scaleFactor = 1f;
				}
				var source = Projectile.GetSource_FromThis();
				Gore gore = Gore.NewGoreDirect(source, Projectile.Center, default(Vector2), Main.rand.Next(61, 64), 1f);
				gore.velocity *= scaleFactor;
				gore.velocity.X += 1f;
				gore.velocity.Y += 1f;
				gore = Gore.NewGoreDirect(source, Projectile.Center, default(Vector2), Main.rand.Next(61, 64), 1f);
				gore.velocity *= scaleFactor;
				gore.velocity.X += -1f;
				gore.velocity.Y += 1f;
				gore = Gore.NewGoreDirect(source, Projectile.Center, default(Vector2), Main.rand.Next(61, 64), 1f);
				gore.velocity *= scaleFactor;
				gore.velocity.X += 1f;
				gore.velocity.Y += -1f;
				gore = Gore.NewGoreDirect(source, Projectile.Center, default(Vector2), Main.rand.Next(61, 64), 1f);
				gore.velocity *= scaleFactor;
				gore.velocity.X += -1f;
				gore.velocity.Y += -1f;
			}
		}
	}
}
