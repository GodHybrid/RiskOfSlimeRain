using RiskOfSlimeRain.Core.ROREffects.Attributes;
using Terraria;

namespace RiskOfSlimeRain.Core.ROREffects.Interfaces
{
	[CanProc]
	public interface IModifyHit : IROREffectInterface
	{
		void ModifyHitNPC(Player player, Item item, NPC target, ref NPC.HitModifiers modifiers);

		void ModifyHitNPCWithProj(Player player, Projectile proj, NPC target, ref NPC.HitModifiers modifiers);
	}
}
