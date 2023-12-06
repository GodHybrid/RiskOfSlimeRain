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
				return "Terraria/Images/Projectile_" + ProjectileID.Fireball;
			}
		}

		public override void SetDefaults()
		{
			Projectile.CloneDefaults(ProjectileID.Fireball);
			Projectile.aiStyle = -1;
			Projectile.timeLeft = 540;
			Projectile.penetrate = 6;
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			Projectile.penetrate--;
			if (Projectile.penetrate <= 0)
			{
				Projectile.Kill();
				return false;
			}
			if (Projectile.velocity.X != oldVelocity.X)
			{
				Projectile.velocity.X = -oldVelocity.X;
			}
			if (Projectile.velocity.Y != oldVelocity.Y)
			{
				Projectile.velocity.Y = -oldVelocity.Y;
			}
			return false;
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White;
		}

		public override void AI()
		{
			Projectile.velocity.Y += Main.expertMode ? 0.18f : 0.165f;
			Projectile.rotation += 0.8f;

			for (int i = 0; i < 2; i++)
			{
				if (Main.rand.NextFloat() < 0.2f) continue;
				Dust dust = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 100);
				dust.scale += 0.4f + Main.rand.NextFloat(0.5f);
				dust.noGravity = true;
				dust.velocity *= 0.2f;
				dust.noLight = true;
			}
		}
	}
}
