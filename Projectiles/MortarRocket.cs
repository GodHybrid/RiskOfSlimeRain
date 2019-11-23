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
	class MortarRocket : ModProjectile
	{
        public override void SetStaticDefaults()
        {
            Main.projFrames[projectile.type] = 1;
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
            Projectile.NewProjectile(projectile.Center, new Vector2(0, 0), mod.ProjectileType("MortarExplosion"), projectile.damage, 5, Main.myPlayer);
            projectile.Kill();
            return false;
        }

        public override void AI()
		{
            projectile.rotation = projectile.velocity.ToRotation();
            projectile.velocity.Y = projectile.velocity.Y + 0.2f; // 0.1f for arrow gravity, 0.4f for knife gravity
            if (projectile.velocity.Y > 15f) // This check implements "terminal velocity". We don't want the projectile to keep getting faster and faster. Past 16f this projectile will travel through blocks, so this check is useful.
            {
                projectile.velocity.Y = 15f;
            }
            foreach (NPC enemy in Main.npc)
            {
                if ((enemy.CanBeChasedBy() || enemy.netID == 488) && enemy.Hitbox.Intersects(projectile.Hitbox))
                {
                    //enemy.StrikeNPC(projectile.damage, 5, 0, false);
                    Projectile.NewProjectile(projectile.Center, new Vector2(0, 0), mod.ProjectileType("MortarExplosion"), projectile.damage, 5, Main.myPlayer);
                    projectile.Kill();
                }
            }
            return;
		}
        
		public override void Kill(int timeLeft)
		{
            
		}
	}
}
