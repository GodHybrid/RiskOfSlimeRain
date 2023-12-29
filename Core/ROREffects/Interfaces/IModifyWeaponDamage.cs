using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Core.ROREffects.Interfaces
{
	public interface IModifyWeaponDamage : IROREffectInterface
	{
		void ModifyWeaponDamage(Player player, Item item, ref StatModifier damage);
	}
}
