using Terraria.ModLoader;

namespace RiskOfSlimeRain.Projectiles
{
	// to investigate: Projectile.Damage, (8843)
	class BundleOfFireworks : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			//Main.projFrames[projectile.type] = 2; //may be more
		}

		public override void SetDefaults()
		{
			projectile.width = 10;
			projectile.height = 8;
			projectile.friendly = true;
			projectile.penetrate = -1;
			projectile.tileCollide = false;
			projectile.timeLeft = 1800;
		}

		public override void AI()
		{
			/*Custom movement code here*/
			{
				//projectile.rotation = projectile.velocity.ToRotation();
				//projectile.velocity.Y = projectile.velocity.Y + 0.2f; // 0.1f for arrow gravity, 0.4f for knife gravity
				//if (projectile.velocity.Y > 15f) // This check implements "terminal velocity". We don't want the projectile to keep getting faster and faster. Past 16f this projectile will travel through blocks, so this check is useful.
				//{
				//	projectile.velocity.Y = 15f;
				//}
				//foreach (NPC enemy in Main.npc)
				//{
				//	if ((enemy.CanBeChasedBy() || enemy.netID == 488) && enemy.Hitbox.Intersects(projectile.Hitbox))
				//	{
				//		//enemy.StrikeNPC(projectile.damage, 5, 0, false);
				//		Projectile.NewProjectile(projectile.Center, new Vector2(0, 0), ModContent.ProjectileType<MortarExplosion>(), projectile.damage, 5, Main.myPlayer);
				//		projectile.Kill();
				//	}
				//}
				//return;
			}
		}

		public override void Kill(int timeLeft)
		{
			//spawn explosion l8r
		}
	}
}
