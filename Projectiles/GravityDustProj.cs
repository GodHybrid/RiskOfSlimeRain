using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Projectiles
{
	public abstract class GravityDustProj: ModProjectile
	{
		public static void NewProjectile<T>(Vector2 position, Vector2 velocity, Action<T> onCreate = null) where T : GravityDustProj
		{
			Projectile p = Projectile.NewProjectileDirect(position, velocity, ModContent.ProjectileType<T>(), 0, 0f, Main.myPlayer);
			if (p.whoAmI < Main.maxProjectiles)
			{
				T t = p.modProjectile as T;

				onCreate?.Invoke(t);
			}
		}

		public int Timer
		{
			get => (int)projectile.ai[0];
			set => projectile.ai[0] = value;
		}

		public abstract int DustType { get; }

		public virtual Color DustColor => Color.White;

		public virtual int DustAlpha => 100;

		public virtual float DustScale => 1f;

		public virtual float DustChance => 1f;

		public virtual int SlowdownStart => 20;

		public virtual float SlowdownX => 0.9f;

		public virtual float Gravity => 0.2f;

		public virtual float VelocityCapY => 2f;

		public override string Texture => "RiskOfSlimeRain/Empty";

		public override void SetDefaults()
		{
			projectile.width = 8;
			projectile.height = 8;
			projectile.penetrate = -1;
			projectile.tileCollide = false;
			projectile.friendly = true;
			projectile.timeLeft = 55;
		}

		public override void AI()
		{
			Movement();
			Visuals();
		}

		private void Movement()
		{
			Timer++;

			if (Timer > SlowdownStart)
			{
				Timer = SlowdownStart;
				//stop spreading
				projectile.velocity.X *= SlowdownX;
				//fall down
				projectile.velocity.Y += Gravity;
			}

			//velocity cap
			if (projectile.velocity.Y > VelocityCapY)
			{
				projectile.velocity.Y = VelocityCapY;
			}
		}

		private void Visuals()
		{
			if (Main.rand.NextFloat() > DustChance) return;
			Dust.NewDustPerfect(projectile.Center, DustType, Vector2.Zero, DustAlpha, DustColor, DustScale);
		}
	}
}
