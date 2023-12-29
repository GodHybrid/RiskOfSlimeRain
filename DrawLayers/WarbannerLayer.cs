using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RiskOfSlimeRain.Core.ROREffects;
using RiskOfSlimeRain.Core.ROREffects.Common;
using RiskOfSlimeRain.Helpers;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.DrawLayers
{
	public class WarbannerLayer : PlayerDrawLayer
	{
		public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
		{
			if (Main.gameMenu) return false;

			Player drawPlayer = drawInfo.drawPlayer;
			if (drawPlayer.dead || drawInfo.shadow != 0f || !drawPlayer.GetRORPlayer().InWarbannerRange)
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
			Player player = drawInfo.drawPlayer;

			Texture2D tex = ModContent.Request<Texture2D>("RiskOfSlimeRain/Textures/Warbanner").Value;
			float drawX = (int)player.Center.X - Main.screenPosition.X;
			float drawY = (int)player.Center.Y + player.gfxOffY - Main.screenPosition.Y;

			Vector2 off = new Vector2(0, -(40 + (42 / 2)));
			SpriteEffects spriteEffects = SpriteEffects.None;
			if (player.gravDir < 0f)
			{
				off.Y = -off.Y;
				spriteEffects = SpriteEffects.FlipVertically;
			}

			drawY -= player.gravDir * (40 + (42 / 2));
			Color color = Color.White;

			if (player.whoAmI == Main.myPlayer)
			{
				RORPlayer mPlayer = player.GetRORPlayer();
				var effect = ROREffectManager.GetEffectOfType<WarbannerEffect>(mPlayer);
				if (effect?.Active ?? false)
				{
					if (effect.WarbannerReadyToDrop)
					{
						if (mPlayer.WarbannerTimer < 30)
						{
							color = Color.Transparent;
						}
					}
				}
			}

			color *= (255 - player.immuneAlpha) / 255f;

			DrawData data = new DrawData(tex, new Vector2(drawX, drawY), null, color, 0, tex.Size() / 2, 1f, spriteEffects, 0)
			{
				ignorePlayerRotation = true
			};
			drawInfo.DrawDataCache.Add(data);
		}
	}
}
