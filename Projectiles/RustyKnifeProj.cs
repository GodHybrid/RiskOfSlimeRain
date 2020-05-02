using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Helpers;
using Terraria;

namespace RiskOfSlimeRain.Projectiles
{
	/// <summary>
	/// This projectile, when dealing damage, won't trigger OnHitNPC by design
	/// </summary>
	public class RustyKnifeProj : StickyProj
	{
		public const int tickAmount = 4;

		public const int StrikeTimerMax = 30;

		public int TimeLeftDefault => tickAmount * StrikeTimerMax + StrikeTimerMax >> 1;

		/// <summary>
		/// Timer for strikes on only that NPC
		/// </summary>
		public int StrikeTimer
		{
			get => (int)projectile.localAI[0];
			set => projectile.localAI[0] = value;
		}

		public bool SetDirection
		{
			get => projectile.localAI[1] == 1f;
			set => projectile.localAI[1] = value ? 1f : 0f;
		}

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Rusty Knife");
			Main.projFrames[projectile.type] = 6;
		}

		public override void SetDefaults()
		{
			base.SetDefaults();
			projectile.Size = new Vector2(26, 34);
			projectile.timeLeft = TimeLeftDefault;
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
			if (!SetDirection)
			{
				projectile.spriteDirection = Main.rand.NextBool().ToDirectionInt();
				SetDirection = true;
			}

			projectile.WaterfallAnimation(5);
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White;
		}
	}
}
