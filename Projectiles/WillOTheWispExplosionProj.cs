using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Projectiles
{
	public class WillOTheWispExplosionProj : ModProjectile, IExcludeOnHit
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Will o' the Wisp's explosion");
			Main.projFrames[projectile.type] = 7;
		}

		public override void SetDefaults()
		{
			base.SetDefaults();
			projectile.Size = new Vector2(106, 184);
			projectile.timeLeft = 26;
			drawOriginOffsetY = (int)(-projectile.height / 2.5f);
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
