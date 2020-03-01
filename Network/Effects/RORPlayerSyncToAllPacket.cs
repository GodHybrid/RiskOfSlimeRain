using WebmilioCommons.Networking;

namespace RiskOfSlimeRain.Network.Effects
{
	//TODO test in MP
	public class RORPlayerSyncToAllPacket : RORPlayerSyncBasePacket
	{
		public override NetworkPacketBehavior Behavior => NetworkPacketBehavior.SendToAll;

		public RORPlayerSyncToAllPacket() : base() { }

		public RORPlayerSyncToAllPacket(RORPlayer mPlayer) : base(mPlayer) { }
	}
}
