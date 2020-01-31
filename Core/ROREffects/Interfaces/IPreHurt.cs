using RiskOfSlimeRain.Core.ROREffects.Attributes;
using Terraria;
using Terraria.DataStructures;

namespace RiskOfSlimeRain.Core.ROREffects.Interfaces
{
	[CanProc]
	public interface IPreHurt : IROREffectInterface
	{
		bool PreHurt(Player player, bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource);
	}
}
