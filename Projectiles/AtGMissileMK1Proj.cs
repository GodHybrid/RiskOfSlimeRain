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
		
		public override void FadeIn()
		{
			base.FadeIn();
		}

		public override void SpawnDust()
		{
			base.SpawnDust();
		}

		public override void Kill(int timeLeft)
		{
			if (timeLeft == 0)
			{
				Main.PlaySound(SoundID.Item14.WithVolume(0.6f), projectile.Center);
				Vector2 velo;
				for (int i = 0; i < 4; i++)
				{
					velo = Vector2.UnitX.RotatedBy(-45).RotatedByRandom(270) * 4;
					GravityDustProj.NewProjectile<BundleOfFireworksVisualExplosion>(projectile.Center, velo);
				}
			}
			else
			{
				Main.PlaySound(SoundID.Item14.WithVolume(0.6f), projectile.Center);
				
			}
		}
	}
}
