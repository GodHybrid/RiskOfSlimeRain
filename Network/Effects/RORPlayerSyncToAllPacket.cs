namespace RiskOfSlimeRain.Network.Effects
{
	public class RORPlayerSyncToAllPacket : RORPlayerSyncPacket
	{
		public override bool Broadcast => true;

		public RORPlayerSyncToAllPacket() { }

		public RORPlayerSyncToAllPacket(RORPlayer mPlayer) : base(mPlayer)
		{

		}
	}
}
