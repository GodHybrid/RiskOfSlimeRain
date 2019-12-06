using Terraria;

namespace RiskOfSlimeRain.Effects.Interfaces
{
	public interface IModifyHit : IROREffectInterface
	{
		void ModifyHitNPC(Player player, Item item, NPC target, ref int damage, ref float knockback, ref bool crit);

		void ModifyHitNPCWithProj(Player player, Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection);
	}
}
