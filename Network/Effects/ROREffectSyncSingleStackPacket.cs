using RiskOfSlimeRain.Helpers;
using System.IO;
using Terraria;
using Terraria.ID;

namespace RiskOfSlimeRain.Network.Effects
{
	public class ROREffectSyncSingleStackPacket : PlayerPacket
	{
		public readonly int index;

		public ROREffectSyncSingleStackPacket() { }

		public ROREffectSyncSingleStackPacket(Player player, int index) : base(player)
		{
			this.index = index;
		}

		protected override void PostSend(BinaryWriter writer, Player player)
		{
			writer.Write7BitEncodedInt(index);

			var effect = player.GetRORPlayer().Effects[index];
			effect.NetSendStack(writer);
			//	//GeneralHelper.Print("" + (DateTime.Now.Ticks % 1000) + " sending stack " + Effect);
		}

		protected override void PostReceive(BinaryReader reader, int sender, Player player)
		{
			int index = reader.Read7BitEncodedInt();

			var mPlayer = player.GetRORPlayer();
			var effect = mPlayer.Effects[index];
			effect.NetReceiveStack(reader);

			if (Main.netMode == NetmodeID.Server)
			{
				new ROREffectSyncSingleStackPacket(player, index).Send(from: sender);
			}

			//	//GeneralHelper.Print("" + (DateTime.Now.Ticks % 1000) + " receiving stack " + Effect);
		}
	}
}
