using Microsoft.Xna.Framework;
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

		public static Effect SetupCircleEffect(Vector2 center, int radius, Color color)
		{
			Effect circle = CircleEffect;
			if (circle != null)
			{
				circle.Parameters["ScreenPos"].SetValue(Main.screenPosition);
				circle.Parameters["ScreenDim"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
				circle.Parameters["EntCenter"].SetValue(center);
				circle.Parameters["EdgeColor"].SetValue(color.ToVector4());
				circle.Parameters["BodyColor"].SetValue(Color.Transparent.ToVector4());
				circle.Parameters["Radius"].SetValue(radius);
				circle.Parameters["HpPercent"].SetValue(1f);
				circle.Parameters["ShrinkResistScale"].SetValue(1f / 24f);
			}
			return circle;
		}

		public static void ApplyToScreen(SpriteBatch spriteBatch, Effect effect)
		{
			if (effect == null) return;

			//Apply the shader to the spritebatch from now on
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, effect, Main.GameViewMatrix.TransformationMatrix);

			spriteBatch.Draw(Main.magicPixel, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Transparent);

			//Stop applying the shader, continue normal behavior
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.instance.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
		}

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
