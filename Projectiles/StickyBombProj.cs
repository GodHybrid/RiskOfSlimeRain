using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RiskOfSlimeRain.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Projectiles
{
	public class StickyBombProj : StickyProj
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Sticky Bomb");
			Main.projFrames[projectile.type] = 2;
		}

		public override void SetDefaults()
		{
			base.SetDefaults();
			projectile.Size = new Vector2(16);
			projectile.timeLeft = 120;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			//manually drawing the texture with 78% opacity, with a glowmask applied
			SpriteEffects effects = SpriteEffects.None;
			Texture2D image = ModContent.GetTexture(Texture);
			Rectangle bounds = new Rectangle
			{
				X = 0,
				Y = projectile.frame,
				Width = image.Bounds.Width,
				Height = image.Bounds.Height / Main.projFrames[projectile.type]
			};
			bounds.Y *= bounds.Height; //cause proj.frame only contains the frame number

			Vector2 stupidOffset = projectile.Size / 2 + new Vector2(0f, projectile.gfxOffY - 2);

			spriteBatch.Draw(image, projectile.position - Main.screenPosition + stupidOffset, bounds, lightColor * 0.78f, projectile.rotation, bounds.Size() / 2, projectile.scale, effects, 0f);
			image = ModContent.GetTexture(Texture + "_Glow");
			spriteBatch.Draw(image, projectile.position - Main.screenPosition + stupidOffset, bounds, Color.White, projectile.rotation, bounds.Size() / 2, projectile.scale, effects, 0f);

			return false;
		}

		public int InitTimer
		{
			get => (int)projectile.localAI[1];
			set => projectile.localAI[1] = value;
		}

		public override void WhileStuck(NPC npc)
		{
			if (Main.myPlayer == projectile.owner && projectile.timeLeft <= 3)
			{
				//Explode
				Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<StickyBombExplosion>(), damage, 8, Main.myPlayer);
				projectile.Kill();
				return;
			}

			projectile.LoopAnimation(5);

			if (InitTimer < 8)
			{
				//167 also decent
				if (InitTimer == 0) Main.PlaySound(42, (int)projectile.Center.X, (int)projectile.Center.Y, 166, 0.8f, 0.8f);
				if (InitTimer == 7) Main.PlaySound(42, (int)projectile.Center.X, (int)projectile.Center.Y, 166, 0.8f, 0.6f);
				InitTimer++;
			}
		}
	}
}
