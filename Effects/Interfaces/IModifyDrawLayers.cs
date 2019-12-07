using System.Collections.Generic;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Effects.Interfaces
{
	public interface IModifyDrawLayers : IROREffectInterface
	{
		void ModifyDrawLayers(List<PlayerLayer> layers);
	}
}
