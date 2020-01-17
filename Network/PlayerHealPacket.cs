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

		public int HealAmount
		{
			get => healAmount;
			set => healAmount = value;
		}

		public byte HealWhoAmI
		{
			get => healWhoAmI;
			set => healWhoAmI = value;
		}

		private static byte healWhoAmI = 0;
		private static int healAmount = 0;

		public static void SendPacket(byte healPlayer, int heal)
		{
			healWhoAmI = healPlayer;
			healAmount = heal;
			new PlayerHealPacket().Send();
		}

		protected override bool PostReceive(BinaryReader reader, int fromWho)
		{
			Main.player[HealWhoAmI].HealMe(HealAmount, noBroadcast: true);
			return base.PostReceive(reader, fromWho);
		}
	}
}
