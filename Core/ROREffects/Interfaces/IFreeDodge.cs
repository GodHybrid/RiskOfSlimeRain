using RiskOfSlimeRain.Core.ROREffects.Attributes;
using Terraria;

namespace RiskOfSlimeRain.Core.ROREffects.Interfaces
{
	[CanProc]
	public interface IFreeDodge : IROREffectInterface
	{
		bool FreeDodge(Player player, Player.HurtInfo info);
	}
}
