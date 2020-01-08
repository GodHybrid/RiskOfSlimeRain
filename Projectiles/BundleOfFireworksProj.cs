using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Effects.Interfaces;
using Terraria;
using Terraria.ID;

namespace RiskOfSlimeRain.Projectiles
{
	public class BundleOfFireworksProj : RandomMovementProj, IExcludeOnHit
	{
		public const int explosionCount = 8;

		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();
			Main.projFrames[projectile.type] = 4;
		}

		public override void SetDefaults()
		{
			base.SetDefaults();
			projectile.Size = new Vector2(10);
		}

		public int TextureIndex => projectile.identity % Main.projFrames[projectile.type];

		public bool Spawned
		{
			get => projectile.localAI[1] == 1f;
			set => projectile.localAI[1] = value ? 1f : 0f;
		}

		public override void FadeIn()
		{
			if (!Spawned)
			{
				//Assign texture once on spawn
				projectile.frame = TextureIndex;
				Spawned = true;
			}
			base.FadeIn();
		}

		public override void SpawnDust()
		{
			//Lighting.AddLight(projectile.Center, new Vector3(0.8f, 0.6f, 0.6f));
			base.SpawnDust();
		}

		public override void Kill(int timeLeft)
		{
			Main.PlaySound(SoundID.Item14.WithVolume(0.6f), projectile.Center);
			Vector2 velo;
			for (int i = 0; i < explosionCount; i++)
			{
				velo = Vector2.UnitX.RotatedBy(-45).RotatedByRandom(270) * 4;
				GravityDustProj.NewProjectile<BundleOfFireworksVisualExplosion>(projectile.Center, velo);
			}
		}
	}
}
