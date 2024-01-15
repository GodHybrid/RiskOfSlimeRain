using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Projectiles
{
	/// <summary>
	/// When spawned, moves in the specified direction, slows down, then homes in on player. Uses ai0 and localAI0
	/// </summary>
	public abstract class PlayerBonusProj : ModProjectile
	{
		public int HomingTimer
		{
			get => (int)Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}

		public int StartHomingTimer
		{
			get => (int)Projectile.localAI[0];
			set => Projectile.localAI[0] = value;
		}

		public bool keepSlowingDown = true;

		public virtual int StartHomingTimerMax => 50;

		public virtual float SlowDownFactor => 0.94f;

		public virtual float MaxHomingSpeed => 10f;

		public virtual int MinHomingAccel => 20;

		public virtual int MaxHomingAccel => 60;

		/// <summary>
		/// Owner of the projectile executes this code (syncing required)
		/// </summary>
		public abstract void ApplyBonus(Player target);

		/// <summary>
		/// Return true if a target is found, and targetIndex is set accordingly
		/// </summary>
		public abstract bool FindTarget(out int targetIndex);

		public override void SetDefaults()
		{
			Projectile.Size = new Vector2(16);
			Projectile.penetrate = 1;
			Projectile.tileCollide = false;
			Projectile.alpha = 150;
			Projectile.timeLeft = 950;
			Projectile.netImportant = true;
		}

		public sealed override void AI()
		{
			if (StartHomingTimer > StartHomingTimerMax)
			{
				if (FindTarget(out int targetIndex))
				{
					Player target = Main.player[targetIndex];
					Homing(target);

					if (Projectile.Hitbox.Intersects(target.Hitbox))
					{
						if (Projectile.owner == Main.myPlayer) ApplyBonus(target);
						Projectile.Kill();
						return;
					}
				}
				else
				{
					Projectile.Kill();
				}
			}
			else
			{
				SlowDown();
			}

			OtherAI();
		}

		public virtual void SlowDown()
		{
			//Since this projectile is spawned with high speed
			StartHomingTimer++;
			if (keepSlowingDown)
			{
				float lenSQ = Projectile.velocity.LengthSquared();
				if (lenSQ < 0.5f)
				{
					Projectile.velocity = Vector2.Zero;
					keepSlowingDown = false;
				}
				else
				{
					Projectile.velocity *= SlowDownFactor;
				}
			}
		}

		public virtual void Homing(Player target)
		{
			/*
			 * What happens here is: Initially, the projectile flies in the opposite direction (6 * 30 = 180 degrees)
			 * And then curves towards the destination for 30 ticks
			 */
			HomingTimer++;
			Vector2 direction = target.Center + target.velocity * 5f - Projectile.Center;
			int rot = Utils.Clamp(30 - HomingTimer, 0, 30);
			direction.Normalize();
			direction = direction.RotatedBy(MathHelper.ToRadians(6 * rot * Math.Sign(-direction.X)));
			direction *= MaxHomingSpeed * (1f - (rot / 30f));
			//For that nice initial curving
			float accel = Utils.Clamp(MaxHomingAccel - HomingTimer, MinHomingAccel, MaxHomingAccel);
			Projectile.velocity = (Projectile.velocity * (accel - 1) + direction) / accel;
		}

		public virtual void OtherAI()
		{

		}
	}
}
