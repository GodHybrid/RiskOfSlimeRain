using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Projectiles
{
	/// <summary>
	/// Entirely visual focused
	/// </summary>
	public class PaulsGoatHoofProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Paul's Goat Hoof Trail");
		}

		public override void SetDefaults()
		{
			projectile.Size = new Vector2(40, 2);
			projectile.aiStyle = -1;
			projectile.friendly = true;
			projectile.penetrate = -1;
			projectile.timeLeft = 60;
			projectile.ignoreWater = true;
			projectile.tileCollide = false;
		}

		public override void AI()
		{
			if (projectile.alpha < 255)
			{
				projectile.alpha += 10;
				if (projectile.alpha >= 255)
				{
					projectile.Kill();
				}
			}
		}
	}
}
