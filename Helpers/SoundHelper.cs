using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Helpers
{
	public static class SoundHelper
	{
		#region PlaySound overloads
		public static SoundEffectInstance PlaySound(LegacySoundStyle type, Vector2 position)
		{
			if (type == null)
			{
				return null;
			}
			return PlaySound(type, (int)position.X, (int)position.Y);
		}

		public static SoundEffectInstance PlaySound(LegacySoundStyle type, int x = -1, int y = -1)
		{
			if (type == null)
			{
				return null;
			}
			return PlaySound(type.SoundId, x, y, type.Style, type.Volume, type.GetRandomPitch());
		}

		public static void PlaySound(int type, Vector2 position, int Style = 1)
		{
			PlaySound(type, (int)position.X, (int)position.Y, Style, 1f, 0f);
		}
		#endregion

		public static SoundEffectInstance PlaySound(int type, int x = -1, int y = -1, int Style = 1, float volumeScale = 1, float pitchOffset = 0)
		{
			SoundEffectInstance instance = Main.PlaySound(type, x, y, Style, volumeScale, pitchOffset);

			if (Main.netMode != NetmodeID.MultiplayerClient) return instance;

			//from here, only client
			//Main.NewText(Main.myPlayer.ToString() + " played a sound and broadcasted");
			SendSound(type, x, y, Style, volumeScale, pitchOffset);

			return instance;
		}

		#region Sound Netcode
		public static void HandleBroadcastSound(BinaryReader reader, int whoAmI)
		{
			//only 50 sound types, hence byte
			int type = reader.ReadByte();
			int x = reader.ReadInt32();
			int y = reader.ReadInt32();
			int Style = reader.ReadInt32();
			float volumeScale = reader.ReadSingle();
			float pitchOffset = reader.ReadSingle();

			if (Main.netMode == NetmodeID.Server)
			{
				//forward to other players
				//Console.WriteLine(whoAmI.ToString() + " sent a broadcast request, sending to everyone else");
				SendSound(type, x, y, Style, volumeScale, pitchOffset, -1, whoAmI);
			}
			else
			{
				//Main.NewText(Main.myPlayer.ToString() + " received a sound from " + whoAmI);
				Main.PlaySound(type, x, y, Style, volumeScale, pitchOffset);
			}
		}

		private static void SendSound(int type, int x = -1, int y = -1, int Style = 1, float volumeScale = 1, float pitchOffset = 0, int to = -1, int from = -1)
		{
			ModPacket packet = RiskOfSlimeRain.Instance.GetPacket();
			packet.Write((int)MessageType.BroadcastSound);
			//only 50 sound types, hence byte
			packet.Write((byte)type);
			packet.Write(x);
			packet.Write(y);
			packet.Write(Style);
			packet.Write(volumeScale);
			packet.Write(pitchOffset);
			packet.Send(to, from);
		}
		#endregion
	}
}
