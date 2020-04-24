using WebmilioCommons.Networking;
using WebmilioCommons.Networking.Packets;

namespace RiskOfSlimeRain.Network
{
	/// <summary>
	/// Send to sync the downed boss count from the server to everyone else
	/// </summary>
	public class DownedBossCountPacket : NetworkPacket
	{
		public override NetworkPacketBehavior Behavior => NetworkPacketBehavior.SendToAllClients;

		public int DownedBossCount
		{
			get => RORWorld.downedBossCount;
			set => RORWorld.downedBossCount = value;
		}

		public DownedBossCountPacket() { }
	}
}
