using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Projectiles
{
	class GasBallFire : ModProjectile
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
			projectile.frameCounter = 5;
			projectile.frame = 0;
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
			foreach (NPC enemy in Main.npc)
			{
				if (enemy.Hitbox.Intersects(projectile.Hitbox))
				{
					if (timer == 60)
					{
						enemy.StrikeNPC((int)((0.2f + (projectile.ai[1] * 0.4f)) * projectile.ai[0]), 0, 0);
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
