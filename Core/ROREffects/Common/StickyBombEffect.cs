using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Projectiles;
using Terraria;

namespace RiskOfSlimeRain.Core.ROREffects.Common
{
	public class StickyBombEffect : RORCommonEffect, IOnHit
	{
		const float initial = 1.0f;
		const float increase = 0.4f;

		public override string Description => $"{Chance.ToPercent()} chance to attach a bomb to an enemy, detonating for {(initial + increase).ToPercent()} damage";

		public override string FlavorText => "Once you take the wrapping off, the adhesive is ACTIVE. DON'T TOUCH IT.\nYOU STICK THAT END ON BAD THINGS, NOT YOURSELF";

		public override bool AlwaysProc => false;

		public override float Chance => 0.08f;

		void IOnHit.OnHitNPC(Player player, Item item, NPC target, int damage, float knockback, bool crit)
		{
			SpawnProjectile(player, target);
		}

		void IOnHit.OnHitNPCWithProj(Player player, Projectile proj, NPC target, int damage, float knockback, bool crit)
		{
			SpawnProjectile(player, target);
		}

		void SpawnProjectile(Player player, NPC target)
		{
			int damage = (int)((initial + increase * Stack) * player.GetDamage());
			Vector2 offset = new Vector2(Main.rand.Next(target.width), Main.rand.Next(4, target.height - 4));
			StickyProj.NewProjectile<StickyBombProj>(target, offset, damage);
		}
	}
}
