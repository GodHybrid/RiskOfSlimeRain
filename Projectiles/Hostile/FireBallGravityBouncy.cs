using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Projectiles.Hostile
{
	/// <summary>
	/// Increased gravity in expert mode
	/// </summary>
	public class FireballGravityBouncy : ModProjectile
	{
		public override string Texture
		{
			get
			{
				return "Terraria/Projectile_" + ProjectileID.Fireball;
			}
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Fireball");
		}

		public override void SetDefaults()
		{
			projectile.CloneDefaults(ProjectileID.Fireball);
			projectile.aiStyle = -1;
			projectile.timeLeft = 540;
			projectile.penetrate = 6;
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			projectile.penetrate--;
			if (projectile.penetrate <= 0)
			{
				projectile.Kill();
				return false;
			}
			if (projectile.velocity.X != oldVelocity.X)
			{
				projectile.velocity.X = -oldVelocity.X;
			}
			if (projectile.velocity.Y != oldVelocity.Y)
			{
				projectile.velocity.Y = -oldVelocity.Y;
			}
			return false;
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White;
		}

		public override void AI()
		{
			projectile.velocity.Y += Main.expertMode ? 0.18f : 0.165f;
			projectile.rotation += 0.8f;

			for (int i = 0; i < 2; i++)
			{
				if (Main.rand.NextFloat() < 0.2f) continue;
				Dust dust = Dust.NewDustDirect(projectile.position, projectile.width, projectile.height, DustID.Fire, 0f, 0f, 100);
				dust.scale += 0.4f + Main.rand.NextFloat(0.5f);
				dust.noGravity = true;
				dust.velocity *= 0.2f;
				dust.noLight = true;
			}
		}
	}
}
