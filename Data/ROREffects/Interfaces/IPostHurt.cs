using RiskOfSlimeRain.Data.ROREffects.Attributes;
using Terraria;

namespace RiskOfSlimeRain.Data.ROREffects.Interfaces
{
	[CanProc]
	public interface IPostHurt : IROREffectInterface
	{
		void PostHurt(Player player, bool pvp, bool quiet, double damage, int hitDirection, bool crit);
	}
}
