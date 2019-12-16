using Terraria;

namespace RiskOfSlimeRain.Helpers
{
	public static class ProjectileHelper
	{
		public static void LoopAnimation(this Projectile proj, int speed)
		{
			if (++proj.frameCounter > speed)
			{
				proj.frameCounter = 0;
				if (++proj.frame >= Main.projFrames[proj.type])
				{
					proj.frame = 0;
				}
			}
		}
	}
}
