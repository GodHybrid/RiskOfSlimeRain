using WebmilioCommons.Networking;

namespace RiskOfSlimeRain.Network.Effects
{
	public class RORPlayerSyncToAllClientsPacket : RORPlayerSyncBasePacket
	{
		public override NetworkPacketBehavior Behavior => NetworkPacketBehavior.SendToAllClients;

		public RORPlayerSyncToAllClientsPacket() : base() { }

		public RORPlayerSyncToAllClientsPacket(RORPlayer mPlayer) : base(mPlayer) { }
	}
}
