using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RiskOfSlimeRain.Buffs;
using RiskOfSlimeRain.Effects.Shaders;
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
			//Main.projFrames[projectile.type] = 2;
		}
		public override void SetDefaults()
		{
			projectile.width = 64;
			projectile.height = 96;
			projectile.friendly = false;
			projectile.hostile = false;
			projectile.penetrate = -1;
			projectile.tileCollide = true;
			projectile.timeLeft = 60;
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

		public override void AI()
		{
			projectile.timeLeft = 60;
			Timer++;
			if (Timer > TimerMax)
			{
				Timer = 0;
				Main.player.WhereActive(p => p.DistanceSQ(projectile.Center) < Radius * Radius).Do(p => p.AddBuff(ModContent.BuffType<WarCry>(), 60));
			}
		}
	}
}
