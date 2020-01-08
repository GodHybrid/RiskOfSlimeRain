using RiskOfSlimeRain.Effects.Attributes;
using Terraria;

namespace RiskOfSlimeRain.Effects.Interfaces
{
	[CanProc]
	public interface IPostHurt : IROREffectInterface
	{
		void PostHurt(Player player, bool pvp, bool quiet, double damage, int hitDirection, bool crit);
	}
}
