using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Effects.Interfaces;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Projectiles
{
	public class MortarTubeRocket : ModProjectile, IExcludeOnHit
	{
		public const int maxTimer = 60;

		public const int fanout = 16;

		public const int thickness = 8;

		public int DustTimer
		{
			get => (int)projectile.localAI[1];
			set => projectile.localAI[1] = value;
		}

		public override void SetDefaults()
		{
			projectile.Size = new Vector2(18);
			projectile.friendly = true;
			projectile.penetrate = 1;
			projectile.tileCollide = true;
			projectile.timeLeft = 1800;
			projectile.alpha = 255;
		}

		public override void Kill(int timeLeft)
		{
			if (Main.myPlayer == projectile.owner)
			{
				int damage = (int)projectile.ai[0];
				Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<MortarTubeExplosion>(), damage, projectile.knockBack, Main.myPlayer);
			}
		}

		public override void AI()
		{
			FadeIn();

			Update();

			if (projectile.alpha > 50) return;

			Collide();

			SpawnDust();
		}

		private void FadeIn()
		{
			if (projectile.alpha > 0)
			{
				projectile.alpha -= 35;
				if (projectile.alpha < 0)
				{
					projectile.alpha = 0;
				}
			}
		}

		private void Update()
		{
			projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
			projectile.velocity.Y = projectile.velocity.Y + 0.2f; // 0.1f for arrow gravity, 0.4f for knife gravity
			if (projectile.velocity.Y > 15f) // This check implements "terminal velocity". We don't want the projectile to keep getting faster and faster. Past 16f this projectile will travel through blocks, so this check is useful.
			{
				projectile.velocity.Y = 15f;
			}
		}

		private void Collide()
		{
			for (int i = 0; i < Main.maxNPCs; i++)
			{
				NPC npc = Main.npc[i];
				if ((npc.CanBeChasedBy() || npc.type == NPCID.TargetDummy) && npc.Hitbox.Intersects(projectile.Hitbox))
				{
					projectile.Kill();
					return;
				}
			}
		}

		private void SpawnDust()
		{
			DustTimer = ++DustTimer % maxTimer;
			float sin = (float)Math.Sin(DustTimer * MathHelper.TwoPi / maxTimer) * fanout;

			//Image of sin: https://www.intmath.com/trigonometric-graphs/svg/svgphp-graphs-sine-cosine-amplitude-1-s0.svg
			//First, change the period from 0 to TwoPi, to 0 to 1 by multiplying the argument with TwoPi
			//Then, stretch that period by the maxTimer
			//now, DustTimer is in range of 0 to 60, and our Sin is in range of 0 to 60, so everything is fine
			//lastly, we stretch its return values from -1 to 1, to -fanout to fanout

			Vector2 direction = Vector2.Normalize(projectile.velocity);
			//position of the fin
			Vector2 backOffset = direction * (projectile.height + thickness);
			Vector2 sinDirection;
			Vector2 center;
			Rectangle spawn;

			//two trails
			for (int i = 0; i < 2; i++)
			{
				int sign;
				if (i == 0) sign = -1;
				else sign = 1;
				//one "clockwise", one "counter clockwise", 90 degrees offset to the direction
				sinDirection = direction.RotatedBy(sign * MathHelper.PiOver4) * sin;
				center = projectile.Center - backOffset + sinDirection;
				spawn = Utils.CenteredRectangle(center, new Vector2(thickness));
				//fire
				Dust dust = Dust.NewDustDirect(spawn.TopLeft(), spawn.Height, spawn.Width, 6, 0f, 0f, 100, default(Color), 1f);
				dust.scale *= Main.rand.NextFloat(1f, 2f);
				dust.velocity *= 0.2f;
				dust.noGravity = true;

				//smoke
				dust = Dust.NewDustDirect(spawn.TopLeft(), spawn.Height, spawn.Width, 31, 0f, 0f, 100, default(Color), 0.4f);
				dust.fadeIn = Main.rand.NextFloat(1f, 1.5f);
				dust.velocity *= 0.05f;
			}
		}
	}
}
