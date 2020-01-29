using RiskOfSlimeRain.Data.ROREffects;
using RiskOfSlimeRain.Helpers;
using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using WebmilioCommons.Networking.Packets;

namespace RiskOfSlimeRain.Network.Effects
{
	public abstract class RORPlayerSyncBasePacket : NetworkPacket
	{
		//to do manual syncing via the overrides, you need PreSend to send, and MidReceive to receive

		public int WhoAmI { get; set; }

		public int Count { get; set; }

		private Player Player => Main.player[WhoAmI];

		private RORPlayer ModPlayer => Player.GetModPlayer<RORPlayer>();

		public RORPlayerSyncBasePacket() { }

		public RORPlayerSyncBasePacket(RORPlayer mPlayer)
		{
			//GeneralHelper.Print("Making packet from " + mPlayer.player.whoAmI);
			WhoAmI = mPlayer.player.whoAmI;
			Count = mPlayer.Effects.Count;
		}

		protected override bool PreSend(ModPacket modPacket, int? fromWho = null, int? toWho = null)
		{
			//GeneralHelper.Print(GetType().Name + " " + (DateTime.Now.Ticks % 1000) + " sending " + Player.name + " " + Count + " effects");
			for (int i = 0; i < Count; i++)
			{
				ROREffect effect = ModPlayer.Effects[i];
				effect.SendOnEnter(modPacket);
			}
			return base.PreSend(modPacket, fromWho, toWho);
		}

		protected override bool MidReceive(BinaryReader reader, int fromWho)
		{
			RORPlayer ModPlayer = Player.GetModPlayer<RORPlayer>();
			for (int i = 0; i < Count; i++)
			{
				ROREffect effect = ROREffect.CreateInstanceFromNet(Player, reader);
				ModPlayer.Effects.Add(effect);
			}
			//GeneralHelper.Print(GetType().Name + " " + (DateTime.Now.Ticks % 1000) + " receiving " + Player.name + " " + Count + " effects");
			ROREffectManager.Populate(ModPlayer);
			return base.MidReceive(reader, fromWho);
		}
	}
}
