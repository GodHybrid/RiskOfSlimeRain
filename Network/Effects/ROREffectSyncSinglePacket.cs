using RiskOfSlimeRain.Core.ROREffects;
using RiskOfSlimeRain.Helpers;
using System.IO;
using Terraria;
using Terraria.ID;

namespace RiskOfSlimeRain.Network.Effects
{
	public class ROREffectSyncSinglePacket : PlayerPacket
	{
		public readonly int index;

		public ROREffectSyncSinglePacket() { }

		public ROREffectSyncSinglePacket(Player player, ROREffect effect) : base(player)
		{
			index = ROREffectManager.GetIndexOfEffect(effect);
		}

		protected override void PostSend(BinaryWriter writer, Player player)
		{
			writer.Write7BitEncodedInt(index);
			var mPlayer = player.GetRORPlayer();
			var effect = mPlayer.Effects[index];
			effect.Send(writer);
		}

		protected override void PostReceive(BinaryReader reader, int sender, Player player)
		{
			int index = reader.Read7BitEncodedInt();
			var mPlayer = player.GetRORPlayer();
			var effect = mPlayer.Effects[index];
			effect.Receive(reader);

			if (Main.netMode == NetmodeID.Server)
			{
				new ROREffectSyncSinglePacket(player, effect).Send(from: sender);
			}
		}
	}
}
