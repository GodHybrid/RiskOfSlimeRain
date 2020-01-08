using RiskOfSlimeRain.Helpers;
using System.IO;
using Terraria;
using WebmilioCommons.Networking;
using WebmilioCommons.Networking.Packets;

namespace RiskOfSlimeRain.Network
{
	public class PlayerHealPacket : PlayerNetworkPacket
	{
		public override NetworkPacketBehavior Behavior => NetworkPacketBehavior.SendToAll;

		public int HealAmount
		{
			get => healAmount;
			set => healAmount = value;
		}

		private static int healAmount = 0;

		public static void SendPacket(int heal)
		{
			healAmount = heal;
			new PlayerHealPacket().Send();
		}

		protected override bool PostReceive(BinaryReader reader, int fromWho)
		{
			Player.HealMe(HealAmount, noBroadcast: true);
			return base.PostReceive(reader, fromWho);
		}
	}
}
