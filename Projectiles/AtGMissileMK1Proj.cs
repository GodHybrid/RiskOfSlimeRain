using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using Terraria;
using Terraria.ID;

namespace RiskOfSlimeRain.Projectiles
{
	public class AtGMissileMK1Proj : RandomMovementProj, IExcludeOnHit
	{
		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();
			Main.projFrames[projectile.type] = 3;
			drawOffsetX = 4;
			drawOriginOffsetY = 8;
		}

		public override void SetDefaults()
		{
			base.SetDefaults();
			projectile.Size = new Vector2(16);
		}

		public override void Kill(int timeLeft)
		{
			Main.PlaySound(SoundID.Item14?.WithVolume(0.6f), projectile.Center);
			for (int i = 0; i < 10; i++) //40
			{
				Dust dust = Dust.NewDustDirect(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, DustID.Smoke, 0f, 0f, 100, default(Color), 2f);
				dust.velocity *= 2f; //3f
				if (Main.rand.NextBool(2))
				{
					dust.scale = 0.5f;
					dust.fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
				}
			}
			for (int i = 0; i < 20; i++) //70
			{
				Dust dust = Dust.NewDustDirect(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, DustID.Fire, 0f, 0f, 100, default(Color), 3f);
				dust.noGravity = true;
				dust.velocity *= 4f; //5f
				dust.noLight = true;
				dust = Dust.NewDustDirect(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, DustID.Fire, 0f, 0f, 100, default(Color), 2f);
				dust.velocity *= 2f;
				dust.noLight = true;
			}
		}
	}
}
