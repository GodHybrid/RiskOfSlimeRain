using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Helpers;
using Terraria;

namespace RiskOfSlimeRain.Projectiles
{
	public class RustyKnifeProj : StickyProj
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Rusty Knife");
			Main.projFrames[projectile.type] = 6;
		}

		public override void SetDefaults()
		{
			base.SetDefaults();
			projectile.Size = new Vector2(26, 34);
			projectile.timeLeft = 140;
		}

		private const int StrikeTimerMax = 30;

		//timer for strikes on only that NPC
		public int StrikeTimer
		{
			get => (int)projectile.localAI[0];
			set => projectile.localAI[0] = value;
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
				}
			}
		}

		public override void OtherAI()
		{
			if (projectile.localAI[0] != 1f)
			{
				projectile.spriteDirection = Main.rand.NextBool().ToDirectionInt();
				projectile.localAI[0] = 1f;
			}
			projectile.WaterfallAnimation(5);
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White;
		}
	}
}
