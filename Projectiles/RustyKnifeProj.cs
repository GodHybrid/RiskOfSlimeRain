using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Projectiles
{
	/// <summary>
	/// This projectile, when dealing damage, won't trigger OnHitNPC by design
	/// </summary>
	public class RustyKnifeProj : StickyProj
	{
		public const int tickAmount = 4;

		public const int StrikeTimerMax = 30;

		public int TimeLeftDefault => tickAmount * StrikeTimerMax + StrikeTimerMax / 2;

		/// <summary>
		/// Timer for strikes on only that NPC
		/// </summary>
		public int StrikeTimer
		{
			get => (int)Projectile.localAI[0];
			set => Projectile.localAI[0] = value;
		}

		public bool SetDirection
		{
			get => Projectile.localAI[1] == 1f;
			set => Projectile.localAI[1] = value ? 1f : 0f;
		}

		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 6;
		}

		public override void SetDefaults()
		{
			base.SetDefaults();
			Projectile.Size = new Vector2(26, 34);
			Projectile.timeLeft = TimeLeftDefault;
		}

		public override void WhileStuck(NPC npc)
		{
			if (Main.myPlayer == Projectile.owner)
			{
				StrikeTimer++;
				if (StrikeTimer > StrikeTimerMax && !npc.dontTakeDamage)
				{
					StrikeTimer = 0;
					npc.SimpleStrikeNPC(damage, 0, damageType: ModContent.GetInstance<ArmorPenDamageClass>()); //Does not proc, syncs
				}
			}
		}

		public override void OtherAI()
		{
			if (!SetDirection)
			{
				Projectile.spriteDirection = Main.rand.NextBool().ToDirectionInt();
				SetDirection = true;
			}

			Projectile.WaterfallAnimation(5);
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White;
		}
	}
}
