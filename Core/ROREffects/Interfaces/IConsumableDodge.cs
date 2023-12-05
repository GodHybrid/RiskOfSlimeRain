using RiskOfSlimeRain.Core.ROREffects.Attributes;
using Terraria;

namespace RiskOfSlimeRain.Core.ROREffects.Interfaces
{
	[CanProc]
	public interface IConsumableDodge : IROREffectInterface
	{
		bool ConsumableDodge(Player player, Player.HurtInfo info);
	}
}
