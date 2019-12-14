using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Dusts
{
	/// <summary>
	/// Flies in a straight line, colorable, doesn't glow. Can produce light with the same color as the dust
	/// </summary>
	public class ColorableDust : ModDust
	{
		public override void OnSpawn(Dust dust)
		{
			dust.noGravity = true;
			dust.noLight = true;
		}

		public override bool Update(Dust dust)
		{
			dust.position += dust.velocity;
			dust.scale *= 0.99f;
			if (dust.scale < 0.5f)
			{
				dust.active = false;
			}
			if (!dust.noLight)
			{
				Color color = (Color)GetAlpha(dust, dust.color * 0.5f);
				if (color == null) return false;
				Lighting.AddLight(dust.position, color.ToVector3());
			}
			return false;
		}

		public override Color? GetAlpha(Dust dust, Color lightColor)
		{
			return lightColor * ((255 - dust.alpha) / 255f);
		}
	}
}
