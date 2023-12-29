using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Projectiles
{
	public class HermitsScarfProj : ModProjectile
	{
		public override void SetDefaults()
		{
			Projectile.aiStyle = -1;
			Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Melee;
			Projectile.penetrate = -1;
			//projectile.hide = true;
			Projectile.timeLeft = 150;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
			Projectile.Size = new Vector2(76, 7);
		}

		public override void AI()
		{
			if (Projectile.timeLeft < 50)
			{
				Projectile.alpha += 5;
				if (Projectile.alpha > 250) Projectile.Kill();
			}
		}
	}
}
