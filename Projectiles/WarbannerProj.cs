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

		public override void Kill(int timeLeft)
		{
			Utils.PoofOfSmoke(projectile.Center);
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Effect circle = ShaderManager.CircleEffect;
			if (circle != null)
			{
				circle.Parameters["ScreenPos"].SetValue(Main.screenPosition);
				circle.Parameters["ScreenDim"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
				circle.Parameters["EntCenter"].SetValue(projectile.Center);
				circle.Parameters["EdgeColor"].SetValue(Color.LightGoldenrodYellow.ToVector4() * 0.78f);
				circle.Parameters["BodyColor"].SetValue(Color.Transparent.ToVector4());
				circle.Parameters["Radius"].SetValue(Radius);
				circle.Parameters["HpPercent"].SetValue(1f);
				circle.Parameters["ShrinkResistScale"].SetValue(1f / 24f);

				//Apply the shader to the spritebatch from now on
				spriteBatch.End();
				spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, circle, Main.Transform);
			}

			spriteBatch.Draw(Main.magicPixel, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Transparent);

			if (circle != null)
			{
				//Stop applying the shader, continue normal behavior
				spriteBatch.End();
				spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.Transform);
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
