using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Data
{
	/// <summary>
	/// Class holding the arguments for DrawData
	/// </summary>
	public sealed class PlayerLayerParams
	{
		public Vector2 Offset;

		public Color? Color;

		public bool? IgnoreAlpha;

		public float? Scale;

		private readonly string TexturePath;

		private readonly int? Frame;

		private readonly int? FrameCount;

		public PlayerLayerParams(string texturePath, Vector2 offset = default, Color? color = null, bool? ignoreAlpha = null, float? scale = null, int? frame = null, int? frameCount = null)
		{
			TexturePath = texturePath;
			Offset = offset;
			Color = color;
			IgnoreAlpha = ignoreAlpha;
			Scale = scale;
			Frame = frame;
			FrameCount = frameCount;
		}

		public Texture2D Texture => ModContent.GetTexture("RiskOfSlimeRain/" + TexturePath);

		/// <summary>
		/// Returns the sourceRectangle of the texture. Only vertical spritesheets supported
		/// </summary>
		/// <returns></returns>
		public Rectangle GetFrame()
		{
			Rectangle frame = Texture.Bounds;
			if (FrameCount > 1)
			{
				frame = Texture.Frame(1, FrameCount.Value, 0, Frame ?? 0);
			}
			return frame;
		}

		public override string ToString() => $"{TexturePath}";
	}
}
