using Terraria;

namespace RiskOfSlimeRain.Effects.Interfaces
{
	public interface IGetWeaponCrit : IROREffectInterface
	{
		void GetWeaponCrit(Player player, Item item, ref int crit);
	}
}
