using RiskOfSlimeRain.Helpers;
using System.IO;
using Terraria;
using Terraria.ID;
using WebmilioCommons.Networking;
using WebmilioCommons.Networking.Packets;

namespace RiskOfSlimeRain.Network
{
	public class PlaySoundPacket : NetworkPacket
	{
		public override NetworkPacketBehavior Behavior => NetworkPacketBehavior.SendToAll;

		public byte Type { get; set; }

		public int X { get; set; }

		public int Y { get; set; }

		public int Style { get; set; }

		public float VolumeScale { get; set; }

		public float PitchOffset { get; set; }

		public PlaySoundPacket() { }

		public PlaySoundPacket(int type, int x = -1, int y = -1, int Style = 1, float volumeScale = 1, float pitchOffset = 0)
		{
			Type = (byte)type;
			X = x;
			Y = y;
			this.Style = Style;
			VolumeScale = volumeScale;
			PitchOffset = pitchOffset;
		}

		protected override bool PostReceive(BinaryReader reader, int fromWho)
		{
			if (Main.netMode != NetmodeID.Server)
			{
				Main.PlaySound(Type, X, Y, Style, SoundHelper.FixVolume(VolumeScale), PitchOffset);
			}
			return base.PostReceive(reader, fromWho);
		}
	}
}
