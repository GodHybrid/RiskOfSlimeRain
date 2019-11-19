using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Achievements;

namespace RiskOfSlimeRain.Projectiles
{
	// to investigate: Projectile.Damage, (8843)
	class MeatNugget : ModProjectile
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
            projectile.velocity.Y = projectile.velocity.Y + 0.2f; // 0.1f for arrow gravity, 0.4f for knife gravity
            if (projectile.velocity.Y > 13f) // This check implements "terminal velocity". We don't want the projectile to keep getting faster and faster. Past 16f this projectile will travel through blocks, so this check is useful.
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
