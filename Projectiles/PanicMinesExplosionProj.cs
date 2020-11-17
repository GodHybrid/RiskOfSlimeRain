using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Projectiles
{
	public class PanicMinesExplosionProj : ModProjectile, IExcludeOnHit
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Panic Mine's Explosion");
			Main.projFrames[projectile.type] = 6;
		}

		public override void SetDefaults()
		{
			base.SetDefaults();
			projectile.Size = new Vector2(106, 184);
			projectile.timeLeft = 26;
			drawOriginOffsetY = -projectile.height / 3;
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
