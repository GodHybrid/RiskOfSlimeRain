using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Helpers;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace RiskOfSlimeRain.Projectiles
{
	/// <summary>
	/// When spawned, circles around the target.
	/// </summary>
	public abstract class CirclingMovementProj : ModProjectile
	{
		public virtual int AlphaDecrease => 10;
		public virtual float Radius { get; set; }
		public virtual int Angle { get; set; }
		public virtual Vector2 Position => Vector2.Zero;

		public static void NewProjectile<T>(Vector2 position, int damage, float knockBack, float radius = 30, int angle = 0, Action<T> onCreate = null) where T : CirclingMovementProj
		{
			Projectile p = Projectile.NewProjectileDirect(position, Vector2.Zero, ModContent.ProjectileType<T>(), damage, knockBack, Main.myPlayer, radius, angle);

			if (p.whoAmI < Main.maxProjectiles)
			{
				T t = p.modProjectile as T;

				onCreate?.Invoke(t);
			}
		}

		public override void SetStaticDefaults()
		{
			
		}

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
			Angle = Angle < 360 ? Angle++ : 1;

			FadeIn();

			Movement();

			OtherAI();

			SpawnDust();
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

		public virtual void Movement()
		{
			projectile.Center = Position + new Vector2(Radius, 0).RotatedBy(MathHelper.ToRadians(Angle));
		}

		public virtual void OtherAI()
		{

		}

		public virtual void SpawnDust()
		{ 

		}
	}
}
