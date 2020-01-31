using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Projectiles;
using Terraria;

namespace RiskOfSlimeRain.Core.ROREffects.Common
{
	public class RustyKnifeEffect : RORCommonEffect, IOnHit
	{
		const float initial = 0.15f;

		public override int MaxRecommendedStack => 7;

		public override string Description => $"{initial.ToPercent()} chance to cause bleeding";

		public override string FlavorText => "Murder weapon, case name ELIAS. Probably a lover's spat?\nThere is still dried blood on the knife, so mark it as biological.";

		public override bool AlwaysProc => false;

		public override float Chance => Stack * initial;

		public void OnHitNPC(Player player, Item item, NPC target, int damage, float knockback, bool crit)
		{
			SpawnProjectile(player, target);
		}

		public void OnHitNPCWithProj(Player player, Projectile proj, NPC target, int damage, float knockback, bool crit)
		{
			SpawnProjectile(player, target);
		}

		void SpawnProjectile(Player player, NPC target)
		{
			int damage = (int)(0.35f * player.GetDamage());
			StickyProj.NewProjectile<RustyKnifeProj>(target, damage: damage);
		}
	}
}
