using RiskOfSlimeRain.Core.ROREffects;
using RiskOfSlimeRain.Core.ROREffects.Common;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using WebmilioCommons.Networking;
using WebmilioCommons.Networking.Packets;

namespace RiskOfSlimeRain.Network.Effects
{
	public class ROREffectSyncSingleStackPacket : ModPlayerNetworkPacket<RORPlayer>
	{
		//to do manual syncing via the overrides, you need PreSend to send, and MidReceive to receive
		public override NetworkPacketBehavior Behavior => NetworkPacketBehavior.SendToAll;

		public int Index { get; set; } = -1;

		private ROREffect Effect => ModPlayer.Effects[Index];

		public ROREffectSyncSingleStackPacket() { }

		public ROREffectSyncSingleStackPacket(ROREffect effect)
		{
			Index = ROREffectManager.GetIndexOfEffect(effect);
		}

		protected override bool PreSend(ModPacket modPacket, int? fromWho = null, int? toWho = null)
		{
			if (Index < 0) return false; //In case the parameterless constructor gets used, or index isn't found

			Effect.NetSendStack(modPacket);
			//GeneralHelper.Print("" + (DateTime.Now.Ticks % 1000) + " sending stack " + Effect);
			return base.PreSend(modPacket, fromWho, toWho);
		}

		protected override bool MidReceive(BinaryReader reader, int fromWho)
		{
			Effect.NetReceiveStack(reader);
			//GeneralHelper.Print("" + (DateTime.Now.Ticks % 1000) + " receiving stack " + Effect);
			return base.MidReceive(reader, fromWho);
		}
	}
}
