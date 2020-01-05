using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RiskOfSlimeRain.Buffs;
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
			//spriteBatch.Draw(Main.magicPixel, Main.screenPosition - projectile.Center - new Vector2(projectile.ai[0], projectile.ai[0]) / 2f,
			//					new Rectangle(0, 0, (int)projectile.ai[0], (int)projectile.ai[0]), Color.LightGoldenrodYellow, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
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
				Main.player.WhereActive(p => p.Distance(projectile.Center) < Radius).Do(p => p.AddBuff(ModContent.BuffType<WarCry>(), 60));
			}
		}
	}
}
