using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RiskOfSlimeRain.Core;
using RiskOfSlimeRain.Core.ROREffects;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.DrawLayers
{
	public class AllInOneLayer : PlayerDrawLayer
	{
		public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
		{
			if (Main.gameMenu) return false;

			Player drawPlayer = drawInfo.drawPlayer;
			if (drawPlayer.dead || drawInfo.shadow != 0f)
			{
				return false;
			}

			if (Config.HiddenVisuals(drawPlayer)) return false;

			return true;
		}

		public override Position GetDefaultPosition()
		{
			return new AfterParent(ROREffectManager.ParentLayer);
		}

		protected override void Draw(ref PlayerDrawSet drawInfo)
		{
			Player dPlayer = drawInfo.drawPlayer;

			var effects = ROREffectManager.GetEffectsOf<IPlayerLayer>(dPlayer);
			var allParameters = new List<PlayerLayerParams>();
			foreach (var effect in effects)
			{
				if (effect.Active)
				{
					PlayerLayerParams parameters = ((IPlayerLayer)effect).GetPlayerLayerParams(dPlayer);
					if (parameters != null)
					{
						allParameters.Add(parameters);
					}
				}
			}

			foreach (var parameters in allParameters)
			{
				Texture2D tex = parameters.Texture;
				float drawX = (int)dPlayer.Center.X - Main.screenPosition.X;
				float drawY = (int)dPlayer.Center.Y - Main.screenPosition.Y;

				Vector2 off = parameters.Offset;
				SpriteEffects spriteEffects = SpriteEffects.None;

				if (dPlayer.gravDir < 0f)
				{
					off.Y = -off.Y;
					spriteEffects = SpriteEffects.FlipVertically;
				}
				drawY += off.Y + dPlayer.gfxOffY;
				drawX += off.X;

				Color color = parameters.Color ?? Color.White;
				if (!(parameters.IgnoreAlpha ?? false))
				{
					color *= (255 - dPlayer.immuneAlpha) / 255f;
				}

				Rectangle sourceRect = parameters.GetFrame();

				DrawData data = new DrawData(tex, new Vector2(drawX, drawY), sourceRect, color, 0, sourceRect.Size() / 2, parameters.Scale ?? 1f, spriteEffects, 0)
				{
					ignorePlayerRotation = true
				};
				drawInfo.DrawDataCache.Add(data);
			}
		}
	}
}
