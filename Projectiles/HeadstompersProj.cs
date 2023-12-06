using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Common;
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
			Main.projFrames[Projectile.type] = 8;
		}

		public override void SetDefaults()
		{
			Projectile.Size = new Vector2(22);
			Projectile.scale = 1.5f;
			Projectile.aiStyle = -1;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.penetrate = -1;
			Projectile.hide = true;
			Projectile.timeLeft = 48;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
		}

		public int TargetTopY
		{
			get => (int)Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}

		public int TargetWhoAmI
		{
			get => (int)Projectile.ai[1];
			set => Projectile.ai[1] = value;
		}

		public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
		{
			//if attached to an NPC, draw behind tiles (and the npc) if that NPC is behind tiles, otherwise just behind the NPC.
			int npcIndex = TargetWhoAmI;
			if (npcIndex >= 0 && npcIndex < 200 && Main.npc[npcIndex].active)
			{
				if (Main.npc[npcIndex].behindTiles)
				{
					behindNPCsAndTiles.Add(index);
				}
				else
				{
					behindProjectiles.Add(index);
				}
			}
			else
			{
				behindProjectiles.Add(index);
			}
		}

		public override void AI()
		{
			if (Projectile.localAI[0] != 1f)
			{
				Player player = Projectile.GetOwner();

				if (player.Bottom.Y > TargetTopY)
				{
					player.velocity.Y = -player.velocity.Y * HeadstompersEffect.velocityDecrease;
					Projectile.localAI[0] = 1f;
				}
			}

			Projectile.WaterfallAnimation(3);
			if (Projectile.alpha < 255)
			{
				Projectile.alpha += 6;
				if (Projectile.alpha > 255) Projectile.alpha = 255;
			}
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White * ((255 - Projectile.alpha) / 255f);
		}
	}
}
