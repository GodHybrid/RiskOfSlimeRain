using RiskOfSlimeRain.Core.ROREffects.Attributes;
using Terraria;

namespace RiskOfSlimeRain.Core.ROREffects.Interfaces
{
	/// <summary>
	/// Use this for on-hit only, not for on-kill stuff (use IOnKill instead)
	/// </summary>
	[CanProc]
	public interface IOnHit : IROREffectInterface
	{
		void OnHitNPC(Player player, Item item, NPC target, int damage, float knockback, bool crit);

		void OnHitNPCWithProj(Player player, Projectile proj, NPC target, int damage, float knockback, bool crit);
	}
}
