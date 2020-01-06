using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Effects.Interfaces;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Projectiles
{
	public abstract class BundleOfFireworksProj : RandomMovementProj, IExcludeOnHit
	{
		public const int ExplosionCount = 8;

		public static int RandomFirework => Main.rand.Next(new int[]
		{
			ModContent.ProjectileType<BundleOfFireworks0>(),
			ModContent.ProjectileType<BundleOfFireworks1>(),
			ModContent.ProjectileType<BundleOfFireworks2>(),
			ModContent.ProjectileType<BundleOfFireworks3>()
		});

		public override void SetDefaults()
		{
			base.SetDefaults();
			projectile.Size = new Vector2(10);
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
			for (int i = 0; i <ExplosionCount; i++)
			{
				velo = Vector2.UnitX.RotatedBy(-45).RotatedByRandom(270) * 4;
				GravityDustProj.NewProjectile<BundleOfFireworksVisualExplosion>(projectile.Center, velo);
			}
		}
	}

	#region Firework types
	public class BundleOfFireworks0 : BundleOfFireworksProj
	{

	}

	public class BundleOfFireworks1 : BundleOfFireworksProj
	{

	}

	public class BundleOfFireworks2 : BundleOfFireworksProj
	{

	}

	public class BundleOfFireworks3 : BundleOfFireworksProj
	{

	}
	#endregion
}
