using RiskOfSlimeRain.Core.ROREffects;
using RiskOfSlimeRain.Helpers;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using WebmilioCommons.Networking.Packets;

namespace RiskOfSlimeRain.Network.Effects
{
	/// <summary>
	/// Sends the Effects list, and other various variables
	/// </summary>
	public abstract class RORPlayerSyncBasePacket : NetworkPacket
	{
		//To do manual syncing via the overrides, you need PreSend to send, and MidReceive to receive

		public int WhoAmI { get; set; }

		public int Count { get; set; }

		private Player Player => Main.player[WhoAmI];

		private RORPlayer ModPlayer => Player.GetRORPlayer();

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
			BitsByte flags = new BitsByte();
			flags[0] = ModPlayer.nullifierEnabled;
			flags[1] = ModPlayer.warbannerRemoverDropped;
			modPacket.Write((byte)flags);

			return base.PreSend(modPacket, fromWho, toWho);
		}

		protected override bool MidReceive(BinaryReader reader, int fromWho)
		{
			ModPlayer.Effects.Clear();
			for (int i = 0; i < Count; i++)
			{
				ROREffect effect = ROREffect.CreateInstanceFromNet(Player, reader);
				if (effect == null) return base.MidReceive(reader, fromWho);
				ModPlayer.Effects.Add(effect);
			}
			BitsByte flags = reader.ReadByte();
			ModPlayer.nullifierEnabled = flags[0];
			ModPlayer.warbannerRemoverDropped = flags[1];

			//GeneralHelper.Print(GetType().Name + " " + (DateTime.Now.Ticks % 1000) + " receiving " + Player.name + " " + Count + " effects");
			ROREffectManager.Populate(ModPlayer);

			return base.MidReceive(reader, fromWho);
		}
	}
}
