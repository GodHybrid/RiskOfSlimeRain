using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using RiskOfSlimeRain.Network;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

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

			//From here, only client
			//Main.NewText(Main.myPlayer.ToString() + " played a sound and broadcasted");
			new PlaySoundPacket(type, x, y, Style, volumeScale, pitchOffset).Send();

			return instance;
		}

		/// <summary>
		/// If you use volume above 1f
		/// </summary>
		public static float FixVolume(float volume)
		{
			return Main.soundVolume * volume > 1 ? Main.soundVolume / volume : volume;
		}
	}
}
