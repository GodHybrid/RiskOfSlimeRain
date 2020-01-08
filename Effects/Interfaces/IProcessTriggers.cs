using Terraria;
using Terraria.GameInput;

namespace RiskOfSlimeRain.Effects.Interfaces
{
	public interface IProcessTriggers : IROREffectInterface
	{
		void ProcessTriggers(Player player, TriggersSet triggersSet);
	}
}
