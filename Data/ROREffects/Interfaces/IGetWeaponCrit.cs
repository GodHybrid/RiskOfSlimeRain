using Terraria;

namespace RiskOfSlimeRain.Data.ROREffects.Interfaces
{
	public interface IGetWeaponCrit : IROREffectInterface
	{
		void GetWeaponCrit(Player player, Item item, ref int crit);
	}
}
