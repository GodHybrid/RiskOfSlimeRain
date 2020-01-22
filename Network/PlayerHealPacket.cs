using RiskOfSlimeRain.Helpers;
using System.IO;
using Terraria;
using WebmilioCommons.Networking;
using WebmilioCommons.Networking.Packets;

namespace RiskOfSlimeRain.Network
{
	public class PlayerHealPacket : NetworkPacket
	{
		public override NetworkPacketBehavior Behavior => NetworkPacketBehavior.SendToAll;

		public int HealAmount { get; set; }

		public byte HealWhoAmI { get; set; }

		public PlayerHealPacket() { }

		public PlayerHealPacket(byte whoAmI, int healAmount)
		{
			HealAmount = healAmount;
			HealWhoAmI = whoAmI;
		}

		protected override bool PostReceive(BinaryReader reader, int fromWho)
		{
			Main.player[HealWhoAmI].HealMe(HealAmount, noBroadcast: true);
			return base.PostReceive(reader, fromWho);
		}
	}
}
