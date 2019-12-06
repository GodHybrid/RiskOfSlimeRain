using RiskOfSlimeRain.Effects.Interfaces;
using Terraria;

namespace RiskOfSlimeRain.Effects.Common
{
	public class BitterRootEffect : ROREffect, IResetEffects
	{
		const float increase = 0.07895f;

		public override int MaxStack => 38;

		public override string Description => "Permanently increases maximum life by roughly 8%";

		public override string FlavorText => "Biggest. Ginseng. Root. Ever.";

		public void ResetEffects(Player player)
		{
			int increaseAmount = (int)(player.statLifeMax * Stack * increase);
			player.statLifeMax2 += increaseAmount;
		}
	}
}
