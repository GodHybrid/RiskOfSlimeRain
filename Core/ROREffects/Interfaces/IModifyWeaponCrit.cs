using Terraria;

namespace RiskOfSlimeRain.Core.ROREffects.Interfaces
{
	public interface IModifyWeaponCrit : IROREffectInterface
	{
		void ModifyWeaponCrit(Player player, Item item, ref float crit);
	}
}
