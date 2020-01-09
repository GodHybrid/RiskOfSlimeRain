using Terraria;

namespace RiskOfSlimeRain.Effects.Interfaces
{
	public interface IModifyWeaponDamage : IROREffectInterface
	{
		void ModifyWeaponDamage(Player player, Item item, ref float add, ref float mult, ref float flat);
	}
}
