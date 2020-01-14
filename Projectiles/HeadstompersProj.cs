﻿using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Helpers;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Projectiles
{
	/// <summary>
	/// Entirely visual/audio focused projectile
	/// </summary>
	class HeadstompersProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Headstompers");
			Main.projFrames[projectile.type] = 7;
		}

		public override void SetDefaults()
		{
			projectile.Size = new Vector2(22);
			projectile.scale = 3;
			projectile.aiStyle = -1;
			projectile.friendly = true;
			projectile.melee = true;
			projectile.penetrate = -1;
			projectile.hide = true;
			projectile.timeLeft = 48;
			projectile.ignoreWater = true;
			projectile.tileCollide = false;
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
			if (projectile.localAI[0] != 1f)
			{
				Main.PlaySound(SoundID.Splash, (int)projectile.Center.X, (int)projectile.Center.Y, -1, 4f, Main.rand.NextFloat(0.4f, 0.9f));
				projectile.localAI[0] = 1f;
			}

			projectile.LoopAnimation(6);
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White;
		}
	}
}
