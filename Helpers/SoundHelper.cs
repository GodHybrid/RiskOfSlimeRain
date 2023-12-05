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
		/// <summary>
		/// If you use volume above 1f
		/// </summary>
		public static float FixVolume(float volume)
		{
			return Main.soundVolume * volume > 1 ? Main.soundVolume / volume : volume;
		}
	}
}
