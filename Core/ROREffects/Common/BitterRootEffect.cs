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

		public void ResetEffects(Player player)
		{
			int increaseAmount = (int)(player.statLifeMax * Stack * increase);
			player.statLifeMax2 += increaseAmount;
		}
	}
}
