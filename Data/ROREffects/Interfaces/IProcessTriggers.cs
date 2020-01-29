using Terraria;
using Terraria.GameInput;

namespace RiskOfSlimeRain.Data.ROREffects.Interfaces
{
	public interface IProcessTriggers : IROREffectInterface
	{
		void ProcessTriggers(Player player, TriggersSet triggersSet);
	}
}
