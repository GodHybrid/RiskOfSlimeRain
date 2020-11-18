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
		public static void NewProjectile<T>(Vector2 position, Vector2 velocity, Action<T> onCreate = null) where T : PlayerBonusProj
		{
			Projectile p = Projectile.NewProjectileDirect(position, velocity, ModContent.ProjectileType<T>(), 0, 0f, Main.myPlayer);
			if (p.whoAmI < Main.maxProjectiles)
			{
				T t = p.modProjectile as T;

				onCreate?.Invoke(t);
			}
		}

		public int HomingTimer
		{
			get => (int)projectile.ai[0];
			set => projectile.ai[0] = value;
		}

		public int StartHomingTimer
		{
			get => (int)projectile.localAI[0];
			set => projectile.localAI[0] = value;
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
			projectile.Size = new Vector2(16);
			projectile.penetrate = 1;
			projectile.tileCollide = false;
			projectile.alpha = 150;
			projectile.timeLeft = 950;
			projectile.netImportant = true;
		}

		public sealed override void AI()
		{
			if (StartHomingTimer > StartHomingTimerMax)
			{
				if (FindTarget(out int targetIndex))
				{
					Player target = Main.player[targetIndex];
					Homing(target);

					if (projectile.Hitbox.Intersects(target.Hitbox))
					{
						if (projectile.owner == Main.myPlayer) ApplyBonus(target);
						projectile.Kill();
						return;
					}
				}
				else
				{
					projectile.Kill();
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
				float lenSQ = projectile.velocity.LengthSquared();
				if (lenSQ < 0.5f)
				{
					projectile.velocity = Vector2.Zero;
					keepSlowingDown = false;
				}
				else
				{
					projectile.velocity *= SlowDownFactor;
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
			Vector2 direction = target.Center + target.velocity * 5f - projectile.Center;
			int rot = Utils.Clamp(30 - HomingTimer, 0, 30);
			direction.Normalize();
			direction = direction.RotatedBy(MathHelper.ToRadians(6 * rot * Math.Sign(-direction.X)));
			direction *= MaxHomingSpeed * (1f - (rot / 30f));
			//For that nice initial curving
			float accel = Utils.Clamp(MaxHomingAccel - HomingTimer, MinHomingAccel, MaxHomingAccel);
			projectile.velocity = (projectile.velocity * (accel - 1) + direction) / accel;
		}

		public virtual void OtherAI()
		{

		}
	}
}
