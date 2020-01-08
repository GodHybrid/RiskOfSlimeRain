using RiskOfSlimeRain.Dusts;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Projectiles
{
	public class BundleOfFireworksVisualExplosion : GravityDustProj
	{
		public override int DustType => ModContent.DustType<ColorableDustAlphaFade>();

		public override float Gravity => 0.15f;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Firework explosion");
		}

		public override void PostCreateDust(Dust dust)
		{
			dust.customData = new InAndOutData(direction: -1, outSpeed: 14, reduceScale: false);
		}
	}
}
