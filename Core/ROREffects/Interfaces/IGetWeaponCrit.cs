using Terraria;

namespace RiskOfSlimeRain.Core.ROREffects.Interfaces
{
	public interface IGetWeaponCrit : IROREffectInterface
	{
		void GetWeaponCrit(Player player, Item item, ref int crit);
	}
}
