using Terraria;

namespace RiskOfSlimeRain.Helpers
{
	public static class ProjectileHelper
	{
		/// <summary>
		/// Loops through all frames in a set speed from top to bottom and repeats
		/// </summary>
		public static void LoopAnimation(this Projectile proj, int speed)
		{
			proj.frameCounter++;
			if (proj.frameCounter > speed)
			{
				proj.frameCounter = 0;
				proj.frame++;
				if (proj.frame >= Main.projFrames[proj.type])
				{
					proj.frame = 0;
				}
			}
		}

		/// <summary>
		/// Same as LoopAnimation, but stops at the last frame
		/// </summary>
		public static void WaterfallAnimation(this Projectile proj, int speed)
		{
			if (proj.frame != Main.projFrames[proj.type] - 1) proj.LoopAnimation(speed);
		}

		public static Player GetOwner(this Projectile proj)
		{
			return Main.player[proj.owner];
		}
	}
}
