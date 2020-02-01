using RiskOfSlimeRain.Core.ROREffects.Attributes;
using Terraria;

namespace RiskOfSlimeRain.Core.ROREffects.Interfaces
{
	/// <summary>
	/// Called in OnHitNPC(WithProj) when target life is 0.
	/// </summary>
	[CanProc]
	public interface IOnKill : IROREffectInterface
	{
		void OnKillNPC(Player player, Item item, NPC target, int damage, float knockback, bool crit);

		void OnKillNPCWithProj(Player player, Projectile proj, NPC target, int damage, float knockback, bool crit);
	}
}
