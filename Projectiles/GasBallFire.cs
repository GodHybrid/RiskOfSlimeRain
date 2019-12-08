using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Effects.Interfaces;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Projectiles
{
	public class GasBallFire : ModProjectile, IExcludeOnHit
	{
		byte timer = 0;

		public override void SetStaticDefaults()
		{
			Main.projFrames[projectile.type] = 5;
		}

		public override void SetDefaults()
		{
			projectile.width = 84;
			projectile.height = 14;
			projectile.friendly = true;
			projectile.penetrate = -1;
			//projectile.tileCollide = true;
			projectile.timeLeft = 300;
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			projectile.velocity = new Vector2(0f, 0f);
			return false;
		}

		public override void AI()
		{
			projectile.frameCounter++;
			timer++;
			if (projectile.frameCounter >= 8)
			{
				projectile.frameCounter = 0;
				projectile.frame = (projectile.frame + 1) % 5;
			}
			projectile.velocity.Y = 6f;
			for (int m = 0; m < Main.maxNPCs; m++)
			{
				NPC enemy = Main.npc[m];
				if (enemy.active && enemy.Hitbox.Intersects(projectile.Hitbox))
				{
					if (timer == 60)
					{
						enemy.StrikeNPC(projectile.damage, 0, 0);
					}
					enemy.AddBuff(BuffID.OnFire, 5);
				}
			}
			if (timer == 60) timer = 0;
		}

		public override void Kill(int timeLeft)
		{

		}
	}
}
