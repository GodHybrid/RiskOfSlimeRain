using Terraria;

namespace RiskOfSlimeRain.Effects.Interfaces
{
	public interface IPostHurt : IROREffectInterface
	{
		void PostHurt(Player player, bool pvp, bool quiet, double damage, int hitDirection, bool crit);
	}
}
