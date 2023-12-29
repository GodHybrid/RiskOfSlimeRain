using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Effects
{
	/// <summary>
	/// Responsible for dealing with shaders
	/// </summary>
	[Autoload(Side = ModSide.Client)]
	public class ShaderManager : ModSystem
	{
		public static Effect CircleEffect { get; private set; }

		public static Effect SetupCircleEffect(Vector2 center, int radius, Color edgeColor, Color bodyBolor = default)
		{
			Effect circle = CircleEffect;
			if (circle != null)
			{
				circle.Parameters["ScreenPos"].SetValue(Main.screenPosition);
				circle.Parameters["ScreenDim"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
				circle.Parameters["EntCenter"].SetValue(center);
				circle.Parameters["EdgeColor"].SetValue(edgeColor.ToVector4());
				circle.Parameters["BodyColor"].SetValue((bodyBolor == default ? Color.Transparent : bodyBolor).ToVector4());
				circle.Parameters["Radius"].SetValue(radius);
				circle.Parameters["HpPercent"].SetValue(1f);
				circle.Parameters["ShrinkResistScale"].SetValue(1f / 24f);
			}
			return circle;
		}

		public static void ApplyToScreenOnce(SpriteBatch spriteBatch, Effect effect, bool restore = true)
		{
			if (effect == null) return;

			//Apply the shader to the spritebatch from now on
			StartEffectOnSpriteBatch(spriteBatch, effect);

			DrawEmptyCanvasToScreen(spriteBatch);

			//Normally you want to apply the shader once, and restore. This is for when you repeatedly apply a different effect without restoring vanilla
			if (restore)
			{
				//Stop applying the shader, continue normal behavior
				RestoreVanillaSpriteBatchSettings(spriteBatch);
			}
		}

		public static void StartEffectOnSpriteBatch(SpriteBatch spriteBatch, Effect effect)
		{
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, effect, Main.GameViewMatrix.TransformationMatrix);
		}

		public static void DrawEmptyCanvasToScreen(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Transparent);
		}

		public static void RestoreVanillaSpriteBatchSettings(SpriteBatch spriteBatch)
		{
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
		}

		public override void OnModLoad()
		{
			CircleEffect = RiskOfSlimeRainMod.Instance.Assets.Request<Effect>("Effects/CircleShader/Circle", AssetRequestMode.ImmediateLoad).Value;
		}

		public override void OnModUnload()
		{
			CircleEffect = null;
		}
	}
}
