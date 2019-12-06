using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Buffs;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Projectiles
{
	//ai0 is used to set timeLeft
	public class SpikestripStrip : ModProjectile
	{
		public override void SetDefaults()
		{
			projectile.width = 34;
			projectile.height = 4;
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
			if (projectile.localAI[0] == 0f)
			{
				projectile.localAI[0] = 1f;
				projectile.timeLeft = (int)projectile.ai[0];
			}

			projectile.velocity.Y = 10f;
			for (int m = 0; m < Main.maxNPCs; m++)
			{

				NPC enemy = Main.npc[m];
				if (enemy.Hitbox.Intersects(projectile.Hitbox))
				{
					enemy.AddBuff(ModContent.BuffType<SpikestripSlowdown>(), 60);
				}
			}
		}
	}
}
