using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Helpers;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace RiskOfSlimeRain.Projectiles
{
	/// <summary>
	/// When spawned, randomly moves about in the direction it is shot at, then starts homing in on closest target. ai0 is the random seed, ai1 and localAI0 is used
	/// </summary>
	public abstract class RandomMovementProj : ModProjectile
	{
		private UnifiedRandom rng;

		public UnifiedRandom Rng
		{
			get
			{
				if (rng == null)
				{
					rng = new UnifiedRandom(RandomSeed / (1 + Projectile.identity));
				}
				return rng;
			}
		}

		public int RandomSeed
		{
			get => (int)Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}

		public int HomingTimer
		{
			get => (int)Projectile.ai[1];
			set => Projectile.ai[1] = value;
		}

		public int RandomMoveTimer
		{
			get => (int)Projectile.localAI[0];
			set => Projectile.localAI[0] = value;
		}

		//Status flag for when default acceleration takes place
		public bool keepIncreasingVelocity = true;

		//Set to true to enable accelerating when keepIncreasingVelocity is false
		public bool doKeepIncreasingVelocity = true;

		public virtual int RandomMoveTimerMax => 60;

		public virtual int AlphaCutoff => 50;

		public virtual int AlphaDecrease => 35;

		public virtual int DustBoxThickness => 6;

		public virtual int RandomMoveDirectionChangeFrequency => 4;

		public virtual double RandomMoveDirectionChangeMagnitude => 0.3;

		public virtual float MaxHomingSpeed => 8f;

		public virtual int MaxHomingRangeSQ => 1080 * 1080;

		public virtual int MinHomingAccel => 4;

		public virtual int MaxHomingAccel => 30;

		public override void SetStaticDefaults()
		{

		}

		public override void SetDefaults()
		{
			Projectile.Size = new Vector2(16);
			Projectile.penetrate = 1;
			Projectile.tileCollide = false;
			Projectile.timeLeft = 300;
			Projectile.alpha = 255;
			Projectile.netImportant = true;
		}

		public override void OnSpawn(IEntitySource source)
		{
			RandomSeed = (int)DateTime.Now.Ticks;
		}

		public sealed override void AI()
		{
			FadeIn();

			if (RandomMoveTimer > RandomMoveTimerMax && FindTarget(out int targetIndex))
			{
				//once it found a target it becomes "able to damage stuff"
				Projectile.friendly = true;
				Homing(targetIndex);
			}
			else
			{
				RandomMovement();
			}

			Movement();

			OtherAI();

			if (Projectile.alpha > AlphaCutoff) return;

			SpawnDust();
		}

		/// <summary>
		/// Return true if a target is found, and targetIndex is set accordingly
		/// </summary>
		public virtual bool FindTarget(out int targetIndex)
		{
			targetIndex = -1;
			float minDistance = float.MaxValue;
			for (int i = 0; i < Main.maxNPCs; i++)
			{
				NPC npc = Main.npc[i];
				if (npc.CanBeChasedBy())
				{
					float betweenProj = Vector2.DistanceSquared(npc.Center, Projectile.Center);
					float betweenPlayer = Vector2.DistanceSquared(npc.Center, Projectile.GetOwner().Center);
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
			if (Projectile.alpha > 0)
			{
				Projectile.alpha -= AlphaDecrease;
				if (Projectile.alpha < 0)
				{
					Projectile.alpha = 0;
				}
			}
		}

		public virtual void RandomMovement()
		{
			if (!keepIncreasingVelocity)
			{
				doKeepIncreasingVelocity = true;
			}

			RandomMoveTimer++;
			if (RandomMoveTimer % RandomMoveDirectionChangeFrequency == 0)
			{
				Projectile.velocity = Projectile.velocity.RotatedBy((Rng.NextDouble() - 0.5) * RandomMoveDirectionChangeMagnitude);
			}
		}

		public virtual void Homing(int targetIndex)
		{
			HomingTimer++;
			Vector2 direction = Main.npc[targetIndex].Center + Main.npc[targetIndex].velocity * 5f - Projectile.Center;
			direction.Normalize();
			direction *= MaxHomingSpeed;
			//For that nice initial curving
			//Accel starts at 30, then goes down to 4
			float accel = Utils.Clamp(MaxHomingAccel - HomingTimer, MinHomingAccel, MaxHomingAccel);
			Projectile.velocity = (Projectile.velocity * (accel - 1) + direction) / accel;
		}

		public virtual void Movement()
		{
			//Since this projectile is spawned with low speed
			if (doKeepIncreasingVelocity)
			{
				doKeepIncreasingVelocity = false;
				keepIncreasingVelocity = true;
			}

			if (keepIncreasingVelocity)
			{
				if (Projectile.velocity.LengthSquared() < MaxHomingSpeed * MaxHomingSpeed)
				{
					Projectile.velocity *= 1.04f;
				}
				else
				{
					keepIncreasingVelocity = false;
				}
			}

			//Terminal velocity cap
			if (Math.Abs(Projectile.velocity.X) > 16) Projectile.velocity.X *= 0.95f;
			if (Math.Abs(Projectile.velocity.Y) > 16) Projectile.velocity.Y *= 0.95f;

			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
		}

		public virtual void SpawnDust()
		{
			Vector2 direction = Vector2.Normalize(Projectile.velocity);
			//Position of the fin
			Vector2 backOffset = direction * Projectile.height;
			Vector2 center = Projectile.Center - backOffset;
			Rectangle spawn = Utils.CenteredRectangle(center, new Vector2(DustBoxThickness));

			Dust dust = Dust.NewDustDirect(spawn.TopLeft(), spawn.Height, spawn.Width, DustID.Torch, 0f, 0f, 100);
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
