using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Projectiles
{
	public class StickyBombProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Sticky Bomb");
			Main.projFrames[projectile.type] = 2;
		}

		public override void SetDefaults()
		{
			projectile.width = 16;
			projectile.height = 16;
			projectile.aiStyle = -1;
			projectile.friendly = true;
			projectile.melee = true;
			projectile.penetrate = 3;
			projectile.hide = true;
			projectile.timeLeft = 120;
			projectile.ignoreWater = true; //make sure the projectile ignores water
			projectile.tileCollide = false; //make sure the projectile doesn't collide with tiles anymore
		}

		public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI)
		{
			//if attached to an NPC, draw behind tiles (and the npc) if that NPC is behind tiles, otherwise just behind the NPC.
			int npcIndex = TargetWhoAmI;
			if (npcIndex >= 0 && npcIndex < 200 && Main.npc[npcIndex].active)
			{
				if (Main.npc[npcIndex].behindTiles)
				{
					drawCacheProjsBehindNPCsAndTiles.Add(index);
				}
				else
				{
					drawCacheProjsBehindProjectiles.Add(index);
				}
			}
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

		//packed offset
		public uint PackedOffset
		{
			get => (uint)projectile.ai[0];
			set => projectile.ai[0] = value;
		}

		//index of the current target
		public int TargetWhoAmI
		{
			get => (int)projectile.ai[1];
			set => projectile.ai[1] = value;
		}

		public int InitTimer
		{
			get => (int)projectile.localAI[1];
			set => projectile.localAI[1] = value;
		}

		public int Damage => (int)projectile.localAI[0];

		public override void AI()
		{
			Animate();
			StickyAI();
		}

		private void Animate()
		{
			if (++projectile.frameCounter > 5)
			{
				projectile.frameCounter = 0;
				if (++projectile.frame >= Main.projFrames[projectile.type])
				{
					projectile.frame = 0;
				}
			}
		}

		private void StickyAI()
		{
			if (InitTimer < 8)
			{
				//167 also decent
				if (InitTimer == 0) Main.PlaySound(42, (int)projectile.Center.X, (int)projectile.Center.Y, 166, 0.8f, 0.8f);
				if (InitTimer == 7) Main.PlaySound(42, (int)projectile.Center.X, (int)projectile.Center.Y, 166, 0.8f, 0.6f);
				InitTimer++;
			}

			if (Main.myPlayer == projectile.owner && projectile.timeLeft <= 3)
			{
				//Explode
				Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<StickyBombExplosion>(), Damage, 8, Main.myPlayer);
				projectile.Kill();
				return;
			}

			int projTargetIndex = TargetWhoAmI;
			if (projTargetIndex < 0 || projTargetIndex >= 200)
			{
				//if the index is past its limits, kill it
				projectile.Kill();
				return;
			}
			NPC npc = Main.npc[projTargetIndex];
			if (npc.active && !npc.dontTakeDamage)
			{
				//if the target is active and can take damage
				//set the projectile's position relative to the target's position + some offset

				projectile.Center = npc.position + GetOffset();
				projectile.gfxOffY = npc.gfxOffY;
			}
			else
			{
				//otherwise, kill the projectile
				projectile.Kill();
			}
		}

		private Vector2 GetOffset()
		{
			uint offY = PackedOffset & 0xFFFF;
			uint offX = (PackedOffset >> 16) & 0xFFFF;
			return new Vector2(offX, offY);
		}
	}
}
