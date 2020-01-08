using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Effects.Interfaces;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Projectiles;
using Terraria;

namespace RiskOfSlimeRain.Effects.Common
{
	public class BundleOfFireworksEffect : RORCommonEffect, IOnHit
	{
		const float initial = 0.005f;
		const float increase = 0.005f;
		const float damageIncrease = 3f;
		const int fireworkCount = 4;

		public override string Description => $" {(initial + increase).ToPercent()} chance to fire {fireworkCount} fireworks that deal {damageIncrease.ToPercent()} damage";

		public override string FlavorText => "Disguising homing missiles as fireworks? \nDon't ever quote me on it, but it was pretty smart";

		//Original, todo
		//const int initial = 6;
		//const int increase = 2;
		//public override string Description => $"Fire {initial + increase} fireworks that deal {damageIncrease.ToPercent()} damage";
		/*
		 * 
		 for loop up to initial + increase * Stack
			//Vector2 velo = new Vector2(Main.rand.NextFloat(-0.25f, 0.25f), -2f);
			//RandomMovementProj.NewProjectile<BundleOfFireworksProj>(target.Center, velo, damage, 10f);
		 * 
		 */

		public override bool AlwaysProc => false;

		public override float Chance => initial + increase * Stack;

		public void OnHitNPC(Player player, Item item, NPC target, int damage, float knockback, bool crit)
		{
			if (target.life <= 0) SpawnProjectile(player, target);
		}

		public void OnHitNPCWithProj(Player player, Projectile proj, NPC target, int damage, float knockback, bool crit)
		{
			if (target.life <= 0) SpawnProjectile(player, target);
		}

		void SpawnProjectile(Player player, NPC target)
		{
			int damage = (int)(damageIncrease * player.GetDamage());
			for (int i = 0; i < fireworkCount; i++)
			{
				Vector2 velo = new Vector2(Main.rand.NextFloat(-0.25f, 0.25f), -2f);
				RandomMovementProj.NewProjectile<BundleOfFireworksProj>(target.Center, velo, damage, 10f);
			}
		}
	}
}
