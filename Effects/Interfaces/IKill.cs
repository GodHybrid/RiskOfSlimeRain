using RiskOfSlimeRain.Effects.Attributes;
using Terraria;
using Terraria.DataStructures;

namespace RiskOfSlimeRain.Effects.Interfaces
{
	[CanProc]
	public interface IKill : IROREffectInterface
	{
		void Kill(Player player, double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource);
	}
}
