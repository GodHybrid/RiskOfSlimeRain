using RiskOfSlimeRain.Data.ROREffects.Attributes;
using Terraria;
using Terraria.DataStructures;

namespace RiskOfSlimeRain.Data.ROREffects.Interfaces
{
	[CanProc]
	public interface IKill : IROREffectInterface
	{
		void Kill(Player player, double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource);
	}
}
