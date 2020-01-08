using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Effects.Interfaces;
using Terraria;

namespace RiskOfSlimeRain.Projectiles
{
	public class RustyKnifeProj : StickyProj, IOncePerNPC
	{
		public override string Texture => "RiskOfSlimeRain/Empty";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Rusty Knife");
		}

		public override void SetDefaults()
		{
			base.SetDefaults();
			projectile.Size = new Vector2(16);
			projectile.timeLeft = 140;
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

		public override void WhileStuck(NPC npc)
		{
			if (Main.myPlayer == projectile.owner)
			{
				StrikeTimer++;
				if (StrikeTimer > StrikeTimerMax)
				{
					StrikeTimer = 0;
					Player player = Main.player[projectile.owner];
					player.ApplyDamageToNPC(npc, damage, 0f, 0, false);
					//npc.StrikeNPC(Damage, 0, 0, false);
				}
			}
		}

		public override void OtherAI()
		{
			if (InitTimer < 8)
			{
				//spawn dust here, 105
				//spawn a line diagonally across the center of the NPC

				Vector2 center = projectile.Center;
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
