using WebmilioCommons.Networking;

namespace RiskOfSlimeRain.Network.Effects
{
	public class RORPlayerSyncToServerPacket : RORPlayerSyncBasePacket
	{
		public override NetworkPacketBehavior Behavior => NetworkPacketBehavior.SendToServer;

		public RORPlayerSyncToServerPacket() : base() { }

		public RORPlayerSyncToServerPacket(RORPlayer mPlayer) : base(mPlayer) { }
	}
}
