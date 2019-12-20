using RiskOfSlimeRain.Helpers;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace RiskOfSlimeRain.Projectiles
{
	/// <summary>
	/// When spawned, randomly moves about in the direction it is shot at, then starts homing in on closest target
	/// </summary>
	public abstract class RandomMovementProj : ModProjectile
	{
		public UnifiedRandom rng;

		public int RandomSeed
		{
			get => (int)projectile.ai[0];
			set => projectile.ai[0] = value;
		}

		public int HomingTimer
		{
			get => (int)projectile.ai[1];
			set => projectile.ai[1] = value;
		}

		public int RandomMoveTimer
		{
			get => (int)projectile.localAI[0];
			set => projectile.localAI[0] = value;
		}

		public virtual int RandomMoveTimerMax => 60;

		public virtual int AlphaCutoff => 50;

		public virtual int AlphaDecrease => 35;

		public virtual int Thickness => 6;

		public virtual int RandomMoveDirectionChangeFrequency => 4;

		public virtual double RandomMoveDirectionChangeMagnitude => 0.3;

		public virtual float MaxSpeed => 8f;

		public virtual int MaxHomingRangeSQ => 1080 * 1080;

		public virtual int MinAccel => 4;

		public virtual int MaxAccel => 30;

		public override void SetDefaults()
		{
			projectile.Size = new Vector2(16);
			projectile.penetrate = 1;
			projectile.tileCollide = false;
			projectile.timeLeft = 300;
			projectile.alpha = 255;
			projectile.netImportant = true;
		}

		public sealed override void AI()
		{
			FadeIn();

			if (RandomMoveTimer > RandomMoveTimerMax && FindTarget(out int targetIndex))
			{
				//once it found a target it becomes "able to damage stuff"
				projectile.friendly = true;
				Homing(targetIndex);
			}
			else
			{
				RandomMovement();
			}

			Movement();

			OtherAI();

			if (projectile.alpha > AlphaCutoff) return;

			SpawnDust();
		}

		/// <summary>
		/// return true if a target is found, and targetIndex is set accordingly
		/// </summary>
		public virtual bool FindTarget(out int targetIndex)
		{
			targetIndex = -1;
			float minDistance = float.MaxValue;
			for (int i = 0; i < 200; i++)
			{
				NPC npc = Main.npc[i];
				if (npc.CanBeChasedBy())
				{
					float betweenProj = Vector2.DistanceSquared(npc.Center, projectile.Center);
					float betweenPlayer = Vector2.DistanceSquared(npc.Center, projectile.GetOwner().Center);
					float between = Math.Min(betweenPlayer, betweenProj);
					if (between < MaxHomingRangeSQ && between < minDistance)
					{
						minDistance = between;
						targetIndex = i;
					}
				}
			}
			return targetIndex != -1;
		}

		public virtual void FadeIn()
		{
			if (projectile.alpha > 0)
			{
				projectile.alpha -= AlphaDecrease;
				if (projectile.alpha < 0)
				{
					projectile.alpha = 0;
				}
			}
		}

		public virtual void RandomMovement()
		{
			RandomMoveTimer++;
			if (rng == null)
			{
				rng = new UnifiedRandom(RandomSeed);
			}
			if (RandomMoveTimer % RandomMoveDirectionChangeFrequency == 0)
			{
				projectile.velocity = projectile.velocity.RotatedBy((rng.NextDouble() - 0.5) * RandomMoveDirectionChangeMagnitude);
			}
		}

		public virtual void Homing(int targetIndex)
		{
			HomingTimer++;
			Vector2 direction = Main.npc[targetIndex].Center + Main.npc[targetIndex].velocity * 5f - projectile.Center;
			direction.Normalize();
			direction *= MaxSpeed;
			//for that nice initial curving
			//accel starts at 30, then goes down to 4
			float accel = Utils.Clamp(-(HomingTimer - MaxAccel), MinAccel, MaxAccel);
			projectile.velocity = (projectile.velocity * (accel - 1) + direction) / accel;
		}

		public virtual void Movement()
		{
			if (projectile.velocity.LengthSquared() < MaxSpeed * MaxSpeed)
			{
				projectile.velocity *= 1.04f;
			}

			projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
			//projectile.velocity.Y += 0.2f; // 0.1f for arrow gravity, 0.4f for knife gravity
			if (Math.Abs(projectile.velocity.X) > MaxSpeed) projectile.velocity.X *= 0.95f;
			if (Math.Abs(projectile.velocity.Y) > MaxSpeed) projectile.velocity.Y *= 0.95f;
		}

		public virtual void SpawnDust()
		{
			Vector2 direction = Vector2.Normalize(projectile.velocity);
			//position of the fin
			Vector2 backOffset = direction * projectile.height;
			Vector2 center = projectile.Center - backOffset;
			Rectangle spawn = Utils.CenteredRectangle(center, new Vector2(Thickness));

			Dust dust = Dust.NewDustDirect(spawn.TopLeft(), spawn.Height, spawn.Width, DustID.Fire, 0f, 0f, 100);
			dust.scale *= Main.rand.NextFloat(1f, 2f);
			dust.velocity *= 0.2f;
			dust.noGravity = true;
			dust.noLight = true;

			dust = Dust.NewDustDirect(spawn.TopLeft(), spawn.Height, spawn.Width, DustID.Smoke, 0f, 0f, 100);
			dust.fadeIn = Main.rand.NextFloat(1f, 1.5f);
			dust.velocity *= 0.05f;
		}

		public virtual void OtherAI()
		{

		}
	}
}
