using Terraria;
using Terraria.DataStructures;

namespace RiskOfSlimeRain.Effects.Interfaces
{
	public interface IKill : IROREffectInterface
	{
		void Kill(Player player, double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource);
	}
}
