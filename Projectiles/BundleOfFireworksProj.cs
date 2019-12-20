using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Effects.Interfaces;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Projectiles
{
	public abstract class BundleOfFireworksProj : RandomMovementProj, IExcludeOnHit
	{
		public static int RandomFirework => Main.rand.Next(new int[]
		{
			ModContent.ProjectileType<BundleOfFireworks0>(),
			ModContent.ProjectileType<BundleOfFireworks1>(),
			ModContent.ProjectileType<BundleOfFireworks2>(),
			ModContent.ProjectileType<BundleOfFireworks3>()
		});

		private static int RandomKillProj => Main.rand.Next(new int[]
		{
			//226, 271
			ProjectileID.RocketFireworksBoxRed,
			ProjectileID.RocketFireworksBoxBlue,
			ProjectileID.RocketFireworksBoxGreen,
			ProjectileID.RocketFireworksBoxYellow
		});

		public override void SetDefaults()
		{
			base.SetDefaults();
			projectile.Size = new Vector2(10);
		}

		public override void SpawnDust()
		{
			Lighting.AddLight(projectile.Center, new Vector3(0.8f, 0.6f, 0.6f));
			base.SpawnDust();
		}

		public override bool PreKill(int timeLeft)
		{
			//this makes it so the projectile "acts" like a vanilla one when despawning
			//it also makes its radius bigger when exploding, so that's a bonus (wanted or not lol)
			projectile.aiStyle = 34;
			//without aiStyle 34, the type assignment won't do anything
			projectile.type = RandomKillProj;
			return true;
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
