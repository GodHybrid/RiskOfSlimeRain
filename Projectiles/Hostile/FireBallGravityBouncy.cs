using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Projectiles.Hostile
{
	/// <summary>
	/// Increased gravity in expert mode by 50%
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
			projectile.timeLeft = 540;
			projectile.penetrate = -1;
		}

		public override void AI()
		{
			projectile.velocity.Y += Main.expertMode ? 0.015f : 0.01f;
		}
	}
}
