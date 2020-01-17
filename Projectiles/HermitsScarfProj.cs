using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Projectiles
{
	public class HermitsScarfProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Evasion text");
		}
		public override void SetDefaults()
		{
			projectile.aiStyle = -1;
			projectile.friendly = true;
			projectile.melee = true;
			projectile.penetrate = -1;
			//projectile.hide = true;
			projectile.timeLeft = 150;
			projectile.ignoreWater = true;
			projectile.tileCollide = false;
			projectile.Size = new Vector2(76, 7);
		}

		public override void AI()
		{
			if (projectile.timeLeft < 50)
			{
				projectile.alpha += 5;
				if (projectile.alpha > 250) projectile.Kill();
			}
		}
	}
}
