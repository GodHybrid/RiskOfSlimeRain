using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Projectiles
{
	// to investigate: Projectile.Damage, (8843)
	public class MeatNugget : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[projectile.type] = 2;
		}

		public override void SetDefaults()
		{
			// while the sprite is actually bigger than 15x15, we use 15x15 since it lets the projectile clip into tiles as it bounces. It looks better.
			projectile.width = 10;
			projectile.height = 8;
			projectile.friendly = true;
			projectile.penetrate = -1;
			//projectile.tileCollide = true;
			projectile.timeLeft = 1800;

		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			projectile.velocity = Vector2.Zero;
			projectile.frame = 1;
			return false;
		}

		public override void AI()
		{
			projectile.rotation = projectile.velocity.ToRotation();
			projectile.velocity.Y = projectile.velocity.Y + 0.2f;
			if (projectile.velocity.Y > 13f)
			{
				projectile.velocity.Y = 13f;
			}
			for (int i = 0; i < Main.maxPlayers; i++)
			{
				Player player = Main.player[i];
				if (player.active && player.Hitbox.Intersects(projectile.Hitbox))
				{
					//TODO proper MP stuff with the heal and proj kill (latter has to happen on all clients)
					int heals = (int)projectile.ai[0];
					player.HealEffect(heals);
					player.statLife += Math.Min(heals, player.statLifeMax2 - player.statLife);
					projectile.Kill();
					break;
				}

			}
		}
	}
}
