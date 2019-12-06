using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Projectiles
{
	public class FireShieldExplosion : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			//Main.projFrames[projectile.type] = 2;
		}
		public override void SetDefaults()
		{
			projectile.width = 200;
			projectile.height = 200;
			projectile.friendly = true;
			projectile.hostile = false;
			projectile.penetrate = -1;
			projectile.tileCollide = false;
			projectile.alpha = 255;
			projectile.timeLeft = 5;
		}

		public override void AI()
		{
			foreach (NPC enemy in Main.npc)
			{
				if (enemy.CanBeChasedBy() && enemy.Hitbox.Intersects(projectile.Hitbox))
				{
					enemy.StrikeNPC(projectile.damage, projectile.knockBack, 0);
				}
			}
			projectile.Kill();
			return;
		}

		public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			//This is needed for stuff like explosives and things that have a large hitbox, that spawn centered around the player
			hitDirection = target.Center.X < Main.player[projectile.owner].Center.X ? -1 : 1;
		}

		public override void Kill(int timeLeft)
		{
			Main.PlaySound(SoundID.DD2_ExplosiveTrapExplode);
			for (int i = 0; i < 20; i++) //40
			{
				Dust dust = Main.dust[Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, DustID.Smoke, 0f, 0f, 100, default(Color), 2f)];
				dust.velocity *= 2f; //3f
				if (Main.rand.NextBool(2))
				{
					dust.scale = 0.5f;
					dust.fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
				}
			}
			for (int i = 0; i < 35; i++) //70
			{
				Dust dust = Main.dust[Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, DustID.Fire, 0f, 0f, 100, default(Color), 3f)];
				dust.noGravity = true;
				dust.velocity *= 4f; //5f
				dust = Main.dust[Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, DustID.Fire, 0f, 0f, 100, default(Color), 2f)];
				dust.velocity *= 2f;
			}
			for (int i = 0; i < 2; i++) //3
			{
				float scaleFactor10 = 0.33f;
				if (i == 1)
				{
					scaleFactor10 = 0.66f;
				}
				if (i == 2)
				{
					scaleFactor10 = 1f;
				}
				Gore gore = Main.gore[Gore.NewGore(new Vector2(projectile.position.X + (float)(projectile.width / 2) - 24f, projectile.position.Y + (float)(projectile.height / 2) - 24f), default(Vector2), Main.rand.Next(61, 64), 1f)];
				gore.velocity *= scaleFactor10;
				gore.velocity.X += 1f;
				gore.velocity.Y += 1f;
				gore = Main.gore[Gore.NewGore(new Vector2(projectile.position.X + (float)(projectile.width / 2) - 24f, projectile.position.Y + (float)(projectile.height / 2) - 24f), default(Vector2), Main.rand.Next(61, 64), 1f)];
				gore.velocity *= scaleFactor10;
				gore.velocity.X += -1f;
				gore.velocity.Y += 1f;
				gore = Main.gore[Gore.NewGore(new Vector2(projectile.position.X + (float)(projectile.width / 2) - 24f, projectile.position.Y + (float)(projectile.height / 2) - 24f), default(Vector2), Main.rand.Next(61, 64), 1f)];
				gore.velocity *= scaleFactor10;
				gore.velocity.X += 1f;
				gore.velocity.Y += -1f;
				gore = Main.gore[Gore.NewGore(new Vector2(projectile.position.X + (float)(projectile.width / 2) - 24f, projectile.position.Y + (float)(projectile.height / 2) - 24f), default(Vector2), Main.rand.Next(61, 64), 1f)];
				gore.velocity *= scaleFactor10;
				gore.velocity.X += -1f;
				gore.velocity.Y += -1f;
			}
		}
	}
}
