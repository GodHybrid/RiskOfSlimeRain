using RiskOfSlimeRain.Core.ROREffects;
using WebmilioCommons.Networking;
using WebmilioCommons.Networking.Packets;

namespace RiskOfSlimeRain.Network.Effects
{
	//unused
	public class ROREffectSyncSinglePacket : ModPlayerNetworkPacket<RORPlayer>
	{
		public override NetworkPacketBehavior Behavior => NetworkPacketBehavior.SendToAll;

		public int Index { get; set; }

		//ROREffect implements INetworkSerializable
		public ROREffect Effect
		{
			get => ModPlayer.Effects[Index];
			set { }
		}

		public ROREffectSyncSinglePacket() { }

		public ROREffectSyncSinglePacket(RORPlayer mPlayer, ROREffect effect)
		{
			Index = ROREffectManager.GetIndexOfEffect(mPlayer, effect);
		}
	}
}
