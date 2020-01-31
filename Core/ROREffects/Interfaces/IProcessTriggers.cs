using Terraria;
using Terraria.GameInput;

namespace RiskOfSlimeRain.Core.ROREffects.Interfaces
{
	public interface IProcessTriggers : IROREffectInterface
	{
		void ProcessTriggers(Player player, TriggersSet triggersSet);
	}
}
