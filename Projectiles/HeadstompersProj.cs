using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Effects.Common;
using RiskOfSlimeRain.Helpers;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Projectiles
{
	/// <summary>
	/// Headstomper visuals and the velocity jump
	/// </summary>
	class HeadstompersProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Headstompers");
			Main.projFrames[projectile.type] = 8;
		}

		public override void SetDefaults()
		{
			projectile.Size = new Vector2(22);
			projectile.scale = 1.5f;
			projectile.aiStyle = -1;
			projectile.friendly = true;
			projectile.melee = true;
			projectile.penetrate = -1;
			projectile.hide = true;
			projectile.timeLeft = 48;
			projectile.ignoreWater = true;
			projectile.tileCollide = false;
		}

		public int TargetTopY
		{
			get => (int)projectile.ai[0];
			set => projectile.ai[0] = value;
		}

		public int TargetWhoAmI
		{
			get => (int)projectile.ai[1];
			set => projectile.ai[1] = value;
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

		public override void AI()
		{
			int npcIndex = TargetWhoAmI;
			if (npcIndex >= 0 && npcIndex < 200 && Main.npc[npcIndex].active)
			{
				if (projectile.localAI[0] != 1f)
				{
					Player player = projectile.GetOwner();

					if (player.Bottom.Y > TargetTopY)
					{
						player.velocity.Y = -player.velocity.Y * HeadstompersEffect.velocityDecrease;
						projectile.localAI[0] = 1f;
					}
				}
			}

			projectile.WaterfallAnimation(3);
			if (projectile.alpha < 255)
			{
				projectile.alpha += 6;
				if (projectile.alpha > 255) projectile.alpha = 255;
			}
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White * ((255 - projectile.alpha) / 255f);
		}
	}
}
