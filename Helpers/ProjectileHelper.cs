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

		public static Player GetOwner(this Projectile proj)
		{
			return Main.player[proj.owner];
		}
	}
}
