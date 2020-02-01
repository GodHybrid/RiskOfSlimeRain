using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RiskOfSlimeRain.Effects;
using Terraria;

namespace RiskOfSlimeRain.Projectiles
{
	/// <summary>
	/// When spawned, moves in the specified direction, slows down, then homes in on player. Uses ai0 and localAI0
	/// </summary>
	public abstract class PlayerBonusCircleProj : PlayerBonusProj
	{
		public sealed override string Texture => "RiskOfSlimeRain/Projectiles/PlayerBonusCircleProj";

		/// <summary>
		/// Color of the circle surrounding this projectile
		/// </summary>
		public abstract Color Color { get; }

		/// <summary>
		/// Radius of the circle surrounding this projectile (additional, 0 for default)
		/// </summary>
		public virtual int Radius => 0;

		public override Color? GetAlpha(Color lightColor)
		{
			return Color;
		}

		public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			bool iAmLast = false;
			for (int i = Main.maxProjectiles - 1; i >= 0; i--)
			{
				Projectile p = Main.projectile[i];
				if (p.active && p.type == projectile.type)
				{
					if (p.whoAmI == projectile.whoAmI)
					{
						iAmLast = true;
					}
					break;
				}
			}

			if (iAmLast)
			{
				int radius = 8;
				for (int i = 0; i < Main.maxProjectiles; i++)
				{
					Projectile p = Main.projectile[i];
					if (p.active && p.type == projectile.type)
					{
						Effect circle;
						//Outer ring
						circle = ShaderManager.SetupCircleEffect(p.Center, radius + Radius, Color, Color * 0.15f);
						if (circle != null)
						{
							ShaderManager.ApplyToScreenOnce(spriteBatch, circle, restore: false);
						}

						//"Sprite"
						if (Radius > 2)
						{
							circle = ShaderManager.SetupCircleEffect(p.Center, radius, Color);
							if (circle != null)
							{
								ShaderManager.ApplyToScreenOnce(spriteBatch, circle, restore: false);
							}
						}
					}
				}
				ShaderManager.RestoreVanillaSpriteBatchSettings(spriteBatch);
			}
		}
	}
}
