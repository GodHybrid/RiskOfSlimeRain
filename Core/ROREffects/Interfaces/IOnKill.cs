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
		//Split because projectiles follow different proc logic
		void OnKillNPCWithItem(Player player, Item item, NPC target, NPC.HitInfo hit, int damageDone);

		void OnKillNPCWithProj(Player player, Projectile proj, NPC target, NPC.HitInfo hit, int damageDone);
	}
}
