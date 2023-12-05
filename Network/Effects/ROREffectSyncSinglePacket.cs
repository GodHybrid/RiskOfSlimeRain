using RiskOfSlimeRain.Core.ROREffects;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Network.Effects
{
	public class ROREffectSyncSinglePacket /*: ModPlayerNetworkPacket<RORPlayer>*/
	{
		public void Send(int toWho = -1, int fromWho = -1) { }
		//public override NetworkPacketBehavior Behavior => NetworkPacketBehavior.SendToAll;

		public int Index { get; set; } = -1;
		
		//ROREffect implements INetworkSerializable
		//public ROREffect Effect
		//{
		//	get => base.ModPlayer.Effects[Index];
		//	set { }
		//}

		public ROREffectSyncSinglePacket() { }

		public ROREffectSyncSinglePacket(ROREffect effect)
		{
			Index = ROREffectManager.GetIndexOfEffect(effect);
		}

		//protected override bool PreSend(ModPacket modPacket, int? fromWho = null, int? toWho = null)
		//{
		//	if (Index < 0) return false; //In case the parameterless constructor gets used, or index isn't found

		//	return base.PreSend(modPacket, fromWho, toWho);
		//}
	}
}
