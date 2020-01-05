using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace RiskOfSlimeRain.Effects.Shaders
{
	/// <summary>
	/// Responsible for dealing with shaders
	/// </summary>
	public static class ShaderManager
	{
		public static Effect CircleEffect { get; private set; }

		public static void Load()
		{
			if (!Main.dedServ && Main.netMode != 2)
			{
				CircleEffect = RiskOfSlimeRainMod.Instance.GetEffect("Effects/Shaders/CircleShader/Barrier");
			}
		}

		public static void Unload()
		{
			CircleEffect = null;
		}
	}
}
