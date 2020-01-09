using RiskOfSlimeRain.Data;
using Terraria;

namespace RiskOfSlimeRain.Effects.Interfaces
{
	public interface IPlayerLayer : IROREffectInterface
	{
		/// <summary>
		/// Return null if you don't want something drawn at all
		/// </summary>
		PlayerLayerParams GetPlayerLayerParams(Player player);
	}
}
