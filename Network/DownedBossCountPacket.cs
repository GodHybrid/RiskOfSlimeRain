using WebmilioCommons.Networking;
using WebmilioCommons.Networking.Packets;

namespace RiskOfSlimeRain.Network
{
	public class DownedBossCountPacket : NetworkPacket
	{
		public override NetworkPacketBehavior Behavior => NetworkPacketBehavior.SendToAllClients;

		public int WarbannerRemoverDropped
		{
			get => RORWorld.downedBossCount;
			set => RORWorld.downedBossCount = value;
		}

		public DownedBossCountPacket() { }
	}
}
