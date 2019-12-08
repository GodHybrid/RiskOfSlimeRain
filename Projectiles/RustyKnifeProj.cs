using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Projectiles
{
	public class RustyKnifeProj : ModProjectile
	{
		public override string Texture => "RiskOfSlimeRain/Empty";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Rusty Knife");
		}

		public override void SetDefaults()
		{
			projectile.width = 16;
			projectile.height = 16;
			projectile.aiStyle = -1;
			projectile.friendly = true;
			projectile.melee = true;
			projectile.penetrate = -1;
			projectile.hide = true;
			projectile.timeLeft = 140;
			projectile.ignoreWater = true; //make sure the projectile ignores water
			projectile.tileCollide = false; //make sure the projectile doesn't collide with tiles anymore
		}

		public int Damage => (int)projectile.ai[0];

		//index of the current target
		public int TargetWhoAmI
		{
			get => (int)projectile.ai[1];
			set => projectile.ai[1] = value;
		}

		private const int StrikeTimerMax = 30;

		//timer for strikes on only that NPC
		public int StrikeTimer
		{
			get => (int)projectile.localAI[0];
			set => projectile.localAI[0] = value;
		}

		public int InitTimer
		{
			get => (int)projectile.localAI[1];
			set => projectile.localAI[1] = value;
		}

		public override void AI()
		{
			StickyAI();
		}

		private void StickyAI()
		{
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

				projectile.Center = npc.Center;
				projectile.gfxOffY = npc.gfxOffY;

				if (Main.myPlayer == projectile.owner)
				{
					StrikeTimer++;
					if (StrikeTimer > StrikeTimerMax)
					{
						StrikeTimer = 0;
						Player player = Main.player[projectile.owner];
						player.ApplyDamageToNPC(npc, Damage, 0f, 0, false);
						//npc.StrikeNPC(Damage, 0, 0, false);
					}
				}
			}
			else
			{
				//otherwise, kill the projectile
				projectile.Kill();
				return;
			}


			if (InitTimer < 8)
			{
				//spawn dust here, 105
				//spawn a line diagonally across the center of the NPC

				Vector2 center = npc.Center;
				//from InitTimer 0 to 7, "draw" across that line in 7 segments
				Vector2 unit = new Vector2(-1f, 0).RotatedBy(MathHelper.ToRadians(30));
				unit *= 4 * (4 - InitTimer);
				for (int i = 0; i < 5; i++)
				{
					Vector2 pos = center + unit;
					pos += new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f));
					Dust.NewDustPerfect(pos, 105, Vector2.Zero);
				}
				InitTimer++;
			}
		}
	}
}
