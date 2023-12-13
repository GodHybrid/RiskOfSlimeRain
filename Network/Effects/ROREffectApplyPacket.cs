using RiskOfSlimeRain.Core.ROREffects;
using RiskOfSlimeRain.Helpers;
using System.IO;
using Terraria;
using Terraria.ID;

namespace RiskOfSlimeRain.Network.Effects
{
	public class ROREffectApplyPacket : PlayerPacket
	{
		public readonly int id;

		public ROREffectApplyPacket() { }

		public ROREffectApplyPacket(RORPlayer mPlayer, ROREffect effect) : base(mPlayer.Player)
		{
			id = effect.Id;
		}

		protected override void PostSend(BinaryWriter writer, Player player)
		{
			writer.Write7BitEncodedInt(id);
		}

		protected override void PostReceive(BinaryReader reader, int sender, Player player)
		{
			int id = reader.Read7BitEncodedInt();
			var mPlayer = player.GetRORPlayer();

			var effect = ROREffectManager.ApplyEffect(mPlayer, id);

			if (Main.netMode == NetmodeID.Server)
			{
				new ROREffectApplyPacket(mPlayer, effect).Send(from: sender);
			}
		}
	}
}
