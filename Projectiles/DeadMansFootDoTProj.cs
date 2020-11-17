using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Helpers;
using System.IO;
using Terraria;

namespace RiskOfSlimeRain.Projectiles
{
	/// <summary>
	/// This projectile, when dealing damage, won't trigger OnHitNPC by design. It also has dynamic timeleft.
	/// </summary>
	public class DeadMansFootDoTProj : StickyProj
	{
		public override string Texture => "RiskOfSlimeRain/Empty";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Dead Man's DoT");
		}

		public override void SetDefaults()
		{
			base.SetDefaults();
			projectile.Size = new Vector2(8);
			projectile.timeLeft = 420; //Default, changed in OtherAI
		}

		private const int StrikeTimerMax = 30;

		//Timer for strikes on only that NPC
		public int StrikeTimer
		{
			get => (int)projectile.localAI[0];
			set => projectile.localAI[0] = value;
		}

		public ushort TimeLeft { get; set; } //Synced

		public bool appliedTimeLeft = false;

		public override void WhileStuck(NPC npc)
		{
			if (Main.myPlayer == projectile.owner)
			{
				StrikeTimer++;
				if (StrikeTimer > StrikeTimerMax)
				{
					StrikeTimer = 0;
					Player player = projectile.GetOwner();
					player.ApplyDamageToNPC(npc, damage, 0f, 0, false);
				}
			}
		}

		public override void OtherAI()
		{
			if (!appliedTimeLeft && TimeLeft != 0)
			{
				appliedTimeLeft = true;
				projectile.timeLeft = TimeLeft;
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
