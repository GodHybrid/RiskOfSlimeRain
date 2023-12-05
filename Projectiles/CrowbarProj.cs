using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Helpers;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Projectiles
{
	/// <summary>
	/// Entirely visual/audio focused projectile
	/// </summary>
	public class CrowbarProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Crowbar");
			Main.projFrames[Projectile.type] = 4;
		}

		public override void SetDefaults()
		{
			Projectile.Size = new Vector2(26);
			Projectile.aiStyle = -1;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.penetrate = -1;
			Projectile.hide = true;
			Projectile.timeLeft = 14;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
		}

		//index of the current target
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
		}

		public override void AI()
		{
			if (Projectile.localAI[0] != 1f)
			{
				SoundEngine.PlaySound(SoundID.Shatter.WithVolumeScale(0.8f).WithPitchOffset(Main.rand.NextFloat(-0.9f, -0.6f)), Projectile.Center);
				Projectile.spriteDirection = Main.rand.NextBool().ToDirectionInt();
				Projectile.localAI[0] = 1f;
			}

			Projectile.LoopAnimation(3);
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White;
		}
	}
}
