using RiskOfSlimeRain.Effects.Attributes;
using Terraria;

namespace RiskOfSlimeRain.Effects.Interfaces
{
	[CanProc]
	public interface IGetWeaponCrit : IROREffectInterface
	{
		void GetWeaponCrit(Player player, Item item, ref int crit);
	}
}
