using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Helpers;
using Terraria;

namespace RiskOfSlimeRain.Projectiles
{
	public class PlayerHealthProj : PlayerBonusCircleProj
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Player Health Orb");
		}

		public int HealAmount
		{
			get => (int)projectile.ai[1];
			set => projectile.ai[1] = value;
		}

		public override Color Color => Color.Green;

		public override int Radius => HealAmount / 10;

		public override void ApplyBonus(Player target)
		{
			target.HealMe(HealAmount);
		}
	}
}
