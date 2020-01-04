using RiskOfSlimeRain.Effects.Interfaces;
using RiskOfSlimeRain.Helpers;

namespace RiskOfSlimeRain.Effects.Common
{
	public class BundleOfFireworksEffect : ROREffect
	{
		const int initial = 6;
		const int increase = 2;
		const float damageIncrease = 3f;

		public override string Description => $"Fire {initial + increase} fireworks that deal {damageIncrease.ToPercent()} damage";

		public override string FlavorText => "Disguising homing missiles as fireworks? \nDon't ever quote me on it, but it was pretty smart.";
		/*
		 * 
		 for loop up to initial + increase * Stack
			//Projectile.NewProjectile(target.Center, new Vector2(0f, -2f), BundleOfFireworksProj.RandomFirework, player.GetDamage() * 3, 1, Main.myPlayer, (int)DateTime.Now.Ticks);
		 * 
		 */
	}
}
