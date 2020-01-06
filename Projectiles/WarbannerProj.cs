using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RiskOfSlimeRain.Effects.Shaders;
using RiskOfSlimeRain.Helpers;
using Terraria;
using Terraria.ModLoader;
using WebmilioCommons.Tinq;

namespace RiskOfSlimeRain.Projectiles
{
	//ai0 contains the radius
	public class WarbannerProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[projectile.type] = 5;
		}
		public override void SetDefaults()
		{
			projectile.width = 32;
			projectile.height = 50;
			projectile.friendly = false;
			projectile.hostile = false;
			projectile.penetrate = -1;
			projectile.tileCollide = true;
			projectile.timeLeft = 60;
			drawOriginOffsetY = 2;
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			projectile.velocity = Vector2.Zero;
			return false;
		}

		public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
		{
			//so it sticks to platforms
			fallThrough = false;
			return base.TileCollideStyle(ref width, ref height, ref fallThrough);
		}

		public override void Kill(int timeLeft)
		{
			Utils.PoofOfSmoke(projectile.Center);
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Effect circle = ShaderManager.SetupCircleEffect(projectile.Center, Radius, Color.LightGoldenrodYellow * 0.78f);
			if (circle != null)
			{
				ShaderManager.ApplyToScreen(spriteBatch, circle);
			}

			return true;
		}

		public int Radius
		{
			get => (int)projectile.ai[0];
			set => projectile.ai[0] = value;
		}

		public const int TimerMax = 50;

		public int Timer
		{
			get => (int)projectile.ai[1];
			set => projectile.ai[1] = value;
		}

		public bool StoppedAnimating
		{
			get => projectile.localAI[0] == 1;
			set => projectile.localAI[0] = value ? 1f : 0f;
		}

		public override void AI()
		{
			projectile.timeLeft = 60;
			GiveBuff();
			Animate();
		}

		private void GiveBuff()
		{
			Timer++;
			if (Timer > TimerMax)
			{
				Timer = 0;
				Main.player.WhereActive(p => p.DistanceSQ(projectile.Center) < Radius * Radius).Do(p => p.GetRORPlayer().ActivateWarbanner());
			}
		}

		private void Animate()
		{
			if (!StoppedAnimating)
			{
				projectile.LoopAnimation(6);
				if (projectile.frame == Main.projFrames[projectile.type] - 1) //if on last frame, stop animating
				{
					StoppedAnimating = true;
				}
			}
		}
	}
}
