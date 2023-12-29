using System.Collections.Generic;

namespace RiskOfSlimeRain.Projectiles.Hostile
{
	public class FireProjHostile : FireProj
	{
		public override bool IgnoreTimeLeft => true;

		public override void SetDefaults()
		{
			Projectile.width = 4;
			Projectile.height = 8;
			Projectile.penetrate = -1;
			Projectile.friendly = false;
			Projectile.hostile = true;
			Projectile.timeLeft = 120;
			Projectile.hide = true;
		}

		public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
		{
			behindNPCsAndTiles.Add(index);
		}
	}
}
