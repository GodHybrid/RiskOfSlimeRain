namespace RiskOfSlimeRain.Projectiles
{
	public class FireProjHostile : FireProj
	{
		public override void SetDefaults()
		{
			projectile.width = 4;
			projectile.height = 8;
			projectile.penetrate = -1;
			projectile.friendly = false;
			projectile.hostile = true;
			projectile.timeLeft = 120;
		}
	}
}
