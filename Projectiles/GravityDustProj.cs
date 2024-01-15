using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Projectiles
{
	public abstract class GravityDustProj : ModProjectile
	{
		public int Timer
		{
			get => (int)Projectile.ai[0];
			set => Projectile.ai[0] = value;
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
			Projectile.width = 8;
			Projectile.height = 8;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.friendly = true;
			Projectile.timeLeft = 55;
		}

		public sealed override void AI()
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
				Projectile.velocity.X *= SlowdownX;
				//fall down
				Projectile.velocity.Y += Gravity;
			}

			//velocity cap
			if (Projectile.velocity.Y > VelocityCapY)
			{
				Projectile.velocity.Y = VelocityCapY;
			}
		}

		/// <summary>
		/// Use to customize the dust that spawns
		/// </summary>
		public virtual void PostCreateDust(Dust dust)
		{

		}

		private void Visuals()
		{
			if (Main.rand.NextFloat() > DustChance) return;
			Dust dust = Dust.NewDustPerfect(Projectile.Center, DustType, Vector2.Zero, DustAlpha, DustColor, DustScale);
			PostCreateDust(dust);
		}
	}
}
