using RiskOfSlimeRain.Dusts;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Projectiles
{
	public class BundleOfFireworksVisualExplosion : GravityDustProj
	{
		public override int DustType => ModContent.DustType<ColorableDustReduceAlpha>();

		public override float Gravity => 0.15f;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Firework explosion");
		}
	}
}
