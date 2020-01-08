using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Effects.Interfaces
{
	public interface IModifyDrawLayers : IROREffectInterface
	{
		void ModifyDrawLayers(Player player, List<PlayerLayer> layers);
	}
}
