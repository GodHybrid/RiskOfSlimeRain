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
			// DisplayName.SetDefault("Panic Mine's Explosion");
			Main.projFrames[Projectile.type] = 6;
		}

		public override void SetDefaults()
		{
			base.SetDefaults();
			Projectile.Size = new Vector2(106, 184);
			Projectile.timeLeft = 26;
			DrawOriginOffsetY = -Projectile.height / 3;
		}

		public override void AI()
		{
			Projectile.WaterfallAnimation(4);
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White;
		}
	}
}
