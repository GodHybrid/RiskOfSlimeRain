using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Projectiles
{
	/// <summary>
	/// Entirely visual focused
	/// </summary>
	public class PaulsGoatHoofProj : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.Size = new Vector2(40, 2);
			Projectile.aiStyle = -1;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
			Projectile.timeLeft = 60;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
		}

		public override bool? CanCutTiles()
		{
			return false;
		}

		public override void AI()
		{
			if (Projectile.alpha < 255)
			{
				Projectile.alpha += 10;
				if (Projectile.alpha >= 255)
				{
					Projectile.Kill();
				}
			}
		}
	}
}
