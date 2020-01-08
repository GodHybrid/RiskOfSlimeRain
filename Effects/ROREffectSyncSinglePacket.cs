using WebmilioCommons.Networking;
using WebmilioCommons.Networking.Packets;

namespace RiskOfSlimeRain.Effects
{
	public class ROREffectSyncSinglePacket : ModPlayerNetworkPacket<RORPlayer>
	{
		//to do manual syncing via the overrides, you need PreSend to send, and MidReceive to receive
		public override NetworkPacketBehavior Behavior => NetworkPacketBehavior.SendToAll;

		public int Index
		{
			get => ROREffectManager.Index;
			set => ROREffectManager.Index = value;
		}

		//ROREffect implements INetworkSerializable
		public ROREffect Effect
		{
			get => ModPlayer.Effects[Index];
			set { }
		}
	}
}
