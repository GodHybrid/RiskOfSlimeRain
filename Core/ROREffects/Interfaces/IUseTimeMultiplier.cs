using Terraria;

namespace RiskOfSlimeRain.Core.ROREffects.Interfaces
{
	public interface IUseTimeMultiplier : IROREffectInterface
	{
		void UseTimeMultiplier(Player player, Item item, ref float multiplier);
	}
}
