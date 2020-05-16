using System.Collections.Generic;

namespace RiskOfSlimeRain.Projectiles.Hostile
{
	public class FireProjHostile : FireProj
	{
		public override bool IgnoreTimeLeft => true;

		public override void SetDefaults()
		{
			projectile.width = 4;
			projectile.height = 8;
			projectile.penetrate = -1;
			projectile.friendly = false;
			projectile.hostile = true;
			projectile.timeLeft = 120;
			projectile.hide = true;
		}

		public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI)
		{
			drawCacheProjsBehindNPCsAndTiles.Add(index);
		}
	}
}
