using System.IO;
using WebmilioCommons.Networking;
using WebmilioCommons.Networking.Packets;
using Terraria.ModLoader;
using RiskOfSlimeRain.Helpers;
using System;

namespace RiskOfSlimeRain.Effects
{
	public class ROREffectSyncSingleStackPacket : ModPlayerNetworkPacket<RORPlayer>
	{
		//to do manual syncing via the overrides, you need PreSend to send, and MidReceive to receive
		public override NetworkPacketBehavior Behavior => NetworkPacketBehavior.SendToAll;

		public int Index
		{
			get => ROREffectManager.Index;
			set => ROREffectManager.Index = value;
		}

		private ROREffect Effect => ModPlayer.Effects[Index];

		protected override bool PreSend(ModPacket modPacket, int? fromWho = null, int? toWho = null)
		{
			Effect.NetSendStack(modPacket);
			//GeneralHelper.Print("" + (DateTime.Now.Ticks % 1000) + " sending stack " + Effect);
			return base.PreSend(modPacket, fromWho, toWho);
		}

		protected override bool MidReceive(BinaryReader reader, int fromWho)
		{
			if (Index == -1) return base.MidReceive(reader, fromWho);
			Effect.NetReceiveStack(reader);
			//GeneralHelper.Print("" + (DateTime.Now.Ticks % 1000) + " receiving stack " + Effect);
			return base.MidReceive(reader, fromWho);
		}
	}
}