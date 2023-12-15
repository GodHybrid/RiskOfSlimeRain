using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RiskOfSlimeRain.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Projectiles
{
	public class StickyBombProj : StickyProj
	{
		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 2;
		}

		public override void SetDefaults()
		{
			base.SetDefaults();
			Projectile.Size = new Vector2(16);
			Projectile.timeLeft = 120;
		}

		public override bool PreDraw(ref Color lightColor)
		{
			//manually drawing the texture with 78% opacity, with a glowmask applied
			SpriteEffects effects = SpriteEffects.None;
			Texture2D image = ModContent.Request<Texture2D>(Texture).Value;
			Rectangle bounds = new Rectangle
			{
				X = 0,
				Y = Projectile.frame,
				Width = image.Bounds.Width,
				Height = image.Bounds.Height / Main.projFrames[Projectile.type]
			};
			bounds.Y *= bounds.Height; //cause proj.frame only contains the frame number

			Vector2 stupidOffset = Projectile.Size / 2 + new Vector2(0f, Projectile.gfxOffY - 2);

			Main.EntitySpriteDraw(image, Projectile.position - Main.screenPosition + stupidOffset, bounds, lightColor * 0.78f, Projectile.rotation, bounds.Size() / 2, Projectile.scale, effects);
			image = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
			Main.EntitySpriteDraw(image, Projectile.position - Main.screenPosition + stupidOffset, bounds, Color.White, Projectile.rotation, bounds.Size() / 2, Projectile.scale, effects);

			return false;
		}

		public int InitTimer
		{
			get => (int)Projectile.localAI[1];
			set => Projectile.localAI[1] = value;
		}

		public override void WhileStuck(NPC npc)
		{
			if (Main.myPlayer == Projectile.owner && Projectile.timeLeft <= 3)
			{
				//Explode
				Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<StickyBombExplosion>(), damage, 8, Main.myPlayer);
				Projectile.Kill();
				return;
			}

			Projectile.LoopAnimation(5);

			if (InitTimer < 8)
			{
				if (InitTimer == 0) SoundEngine.PlaySound(SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.8f), Projectile.Center);
				if (InitTimer == 7) SoundEngine.PlaySound(SoundID.DD2_CrystalCartImpact.WithPitchOffset(0.6f), Projectile.Center);
				InitTimer++;
			}
		}
	}
}
