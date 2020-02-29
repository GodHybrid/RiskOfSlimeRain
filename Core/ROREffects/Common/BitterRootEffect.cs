using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using Terraria;

namespace RiskOfSlimeRain.Core.ROREffects.Common
{
	public class BitterRootEffect : RORCommonEffect, IResetEffects
	{
		const float increase = 0.07895f;

		public override int MaxRecommendedStack => 38;

		public override string Description => $"Permanently increases maximum life by roughly {increase.ToPercent(0)}";

		public override string FlavorText => "Biggest. Ginseng. Root. Ever.";

		public override string UIInfo => $"Life increase: {GetIncreaseAmount(Player)}";

		public void ResetEffects(Player player)
		{
			player.statLifeMax2 += GetIncreaseAmount(player);
		}

		public int GetIncreaseAmount(Player player)
		{
			return (int)(player.statLifeMax * Stack * increase);
		}
	}
}
