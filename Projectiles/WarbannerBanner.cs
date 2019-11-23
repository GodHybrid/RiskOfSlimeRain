using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.Achievements;
using System.IO;
using Microsoft.Xna.Framework.Graphics;

namespace RiskOfSlimeRain.Projectiles
{
	class WarbannerBanner : ModProjectile
	{
		//float rad = RORWorld.WarbannersList[0].radius;

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
			projectile.velocity = new Vector2(0f, 0f);
			return false;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			spriteBatch.Draw(Main.magicPixel, Main.screenPosition - projectile.Center - new Vector2(projectile.ai[0], projectile.ai[0]) / 2f, 
								new Rectangle(0, 0, (int)projectile.ai[0], (int)projectile.ai[0]), Color.LightGoldenrodYellow, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
			return true;
		}

		public override void AI()
		{
			projectile.timeLeft = 60;
			foreach (Player player in Main.player)
			{
				if (player.active && player.Hitbox.Distance(projectile.position) < projectile.ai[0])
				{
					player.AddBuff(mod.BuffType("WarCry"), 60);
				}
			}
			return;
		}

		public override void Kill(int timeLeft)
		{
			
		}
	}
}
