using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Helpers;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Projectiles
{
	/// <summary>
	/// This projectile, when dealing damage, won't trigger OnHitNPC by design. It also has dynamic timeleft.
	/// </summary>
	public class DeadMansFootDoTProj : StickyProj
	{
		public override string Texture => "RiskOfSlimeRain/Empty";

		public override void SetDefaults()
		{
			base.SetDefaults();
			Projectile.Size = new Vector2(8);
			Projectile.timeLeft = 420; //Default, changed in OtherAI
		}

		private const int StrikeTimerMax = 30;

		//Timer for strikes on only that NPC
		public int StrikeTimer
		{
			get => (int)Projectile.localAI[0];
			set => Projectile.localAI[0] = value;
		}

		public ushort TimeLeft { get; set; } //Synced

		public bool appliedTimeLeft = false;

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
			if (!appliedTimeLeft && TimeLeft != 0)
			{
				appliedTimeLeft = true;
				Projectile.timeLeft = TimeLeft;
			}
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White;
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(TimeLeft);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			TimeLeft = reader.ReadUInt16();
		}
	}
}
