namespace RiskOfSlimeRain.Network.Effects
{
	public class RORPlayerSyncToAllPacket : RORPlayerSyncBasePacket
	{
		//public override NetworkPacketBehavior Behavior => NetworkPacketBehavior.SendToAll;

		public RORPlayerSyncToAllPacket() : base() { }

		public RORPlayerSyncToAllPacket(RORPlayer mPlayer) : base(mPlayer) { }
	}
}
