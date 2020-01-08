using Terraria;

namespace RiskOfSlimeRain.Effects.Interfaces
{
	public interface IUseTimeMultiplier : IROREffectInterface
	{
		void UseTimeMultiplier(Player player, Item item, ref float multiplier);
	}
}
