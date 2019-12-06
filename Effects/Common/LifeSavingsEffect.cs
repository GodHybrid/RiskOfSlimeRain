using RiskOfSlimeRain.Effects.Interfaces;
using Terraria;
using Terraria.ID;

namespace RiskOfSlimeRain.Effects.Common
{
	public class LifeSavingsEffect : ROREffect, IPostUpdateEquips
	{
		const int interval = 180;
		int timer = interval;

		public override string Description => "Generate 1 copper every 3 seconds";

		public override string FlavorText => "hi im billy and heer is money for mom thanks";

		public void PostUpdateEquips(Player player)
		{
			//TODO redo to cumulative counter every 3 seconds, + supress with On.PlaySound and some static bool
			timer--;
			if (timer < 0)
			{
				player.QuickSpawnItem(ItemID.CopperCoin, 1);
				timer = interval / Stack + 1;
			}
		}
	}
}
