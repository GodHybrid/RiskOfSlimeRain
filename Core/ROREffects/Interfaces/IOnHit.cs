using RiskOfSlimeRain.Core.ROREffects.Attributes;
using Terraria;

namespace RiskOfSlimeRain.Core.ROREffects.Interfaces
{
	[CanProc]
	public interface IOnHit : IROREffectInterface
	{
		void OnHitNPC(Player player, Item item, NPC target, int damage, float knockback, bool crit);

		void OnHitNPCWithProj(Player player, Projectile proj, NPC target, int damage, float knockback, bool crit);
	}
}
