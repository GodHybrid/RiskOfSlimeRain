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
			projectile.frameCounter = 2;
			projectile.frame = 0;
			//projectile.tileCollide = true;
			projectile.timeLeft = 1800;

		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			projectile.velocity = new Vector2(0f, 0f);
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
			foreach (Player player in Main.player)
			{
				if (player.active && player.Hitbox.Intersects(projectile.Hitbox))
				{
					int heals = 6 * player.GetModPlayer<RORPlayer>().meatNuggets;
					player.HealEffect(heals);
					player.statLife += Math.Min(heals, player.statLifeMax2 - player.statLife);
					projectile.Kill();
					break;
				}
			}
			//if (projectile.timeLeft < 60) this.projectile.alpha += (int)255 / 60;
			return;
		}

		public override void Kill(int timeLeft)
		{

		}
	}
}
