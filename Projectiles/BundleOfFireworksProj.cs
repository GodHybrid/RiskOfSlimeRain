using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace RiskOfSlimeRain.Projectiles
{
	public class BundleOfFireworksProj : RandomMovementProj, IExcludeOnHit
	{
		public const int explosionCount = 8;

		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();
			Main.projFrames[Projectile.type] = 4;
		}

		public override void SetDefaults()
		{
			base.SetDefaults();
			Projectile.Size = new Vector2(10);
		}

		public int TextureIndex => Projectile.identity % Main.projFrames[Projectile.type];

		public bool Spawned
		{
			get => Projectile.localAI[1] == 1f;
			set => Projectile.localAI[1] = value ? 1f : 0f;
		}

		public override void FadeIn()
		{
			if (!Spawned)
			{
				//Assign texture once on spawn
				Projectile.frame = TextureIndex;
				Spawned = true;
			}
			base.FadeIn();
		}

		public override void SpawnDust()
		{
			//Lighting.AddLight(projectile.Center, new Vector3(0.8f, 0.6f, 0.6f));
			base.SpawnDust();
		}

		public override void OnKill(int timeLeft)
		{
			SoundEngine.PlaySound(SoundID.Item14.WithVolumeScale(0.6f), Projectile.Center);
			Vector2 velo;
			for (int i = 0; i < explosionCount; i++)
			{
				velo = Vector2.UnitX.RotatedBy(-45).RotatedByRandom(270) * 4;
				GravityDustProj.NewProjectile<BundleOfFireworksVisualExplosion>(Projectile.GetSource_FromThis(), Projectile.Center, velo);
			}
		}
	}
}
