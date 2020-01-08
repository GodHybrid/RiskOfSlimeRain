using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Projectiles
{
	public class MeatNugget : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[projectile.type] = 2;
		}

		public override void SetDefaults()
		{
			projectile.width = 10;
			projectile.height = 8;
			projectile.friendly = true;
			projectile.penetrate = -1;
			//projectile.tileCollide = true;
			projectile.timeLeft = 1800;
			drawOriginOffsetY = 2;
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			projectile.velocity = Vector2.Zero;
			projectile.frame = 1;
			return false;
		}

		public int Heal
		{
			get => (int)projectile.ai[0];
			set => projectile.ai[0] = value;
		}

		public override void AI()
		{
			projectile.rotation = projectile.velocity.ToRotation();
			projectile.velocity.Y += 0.2f;
			if (projectile.velocity.Y > 13f)
			{
				projectile.velocity.Y = 13f;
			}
			if (projectile.owner == Main.myPlayer)
			{
				projectile.GetOwner().HealMe(Heal);
			}
		}
	}
}
