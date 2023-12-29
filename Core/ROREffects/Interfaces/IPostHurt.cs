using RiskOfSlimeRain.Core.ROREffects.Attributes;
using Terraria;

namespace RiskOfSlimeRain.Core.ROREffects.Interfaces
{
	[CanProc]
	public interface IPostHurt : IROREffectInterface
	{
		void PostHurt(Player player, Player.HurtInfo info);
	}
}
