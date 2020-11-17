using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Projectiles
{
	public class DeadMansFootExplosionProj : ModProjectile, IExcludeOnHit
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Dead Man's Foot's Explosion");
			Main.projFrames[projectile.type] = 6;
		}

		public override void SetDefaults()
		{
			base.SetDefaults();
			projectile.Size = new Vector2(106, 184);
			projectile.timeLeft = 26;
			drawOriginOffsetY = -50;
		}

		public override void AI()
		{
			projectile.WaterfallAnimation(4);
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White;
		}
	}
}
