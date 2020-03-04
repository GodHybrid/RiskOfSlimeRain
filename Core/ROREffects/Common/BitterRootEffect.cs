using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Dusts;
using RiskOfSlimeRain.Helpers;
using Terraria;

namespace RiskOfSlimeRain.Core.ROREffects.Common
{
	public class BitterRootEffect : RORCommonEffect, IResetEffects, IPostUpdateEquips
	{
		const float increase = 0.07895f;

		public const int initialSpawnTimerMin = 40;

		public const int initialSpawnTimerMax = 100;

		public const int fastSpawnTimerMin = 4;

		public const int fastSpawnTimerMax = 10;

		public const int fastSpawnRate = 3;

		public int initialSpawnTimer = 0;

		public int initialSpawnTimerNext = initialSpawnTimerMax;

		public int fastSpawnTimer = 0;

		public int fastSpawnTimerNext = fastSpawnTimerMax;

		public bool spawning = false;

		public override int MaxRecommendedStack => 38;

		public override bool EnforceMaxStack => true;

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

		public void PostUpdateEquips(Player player)
		{
			if (Config.HiddenVisuals(player)) return;

			if (!spawning)
			{
				initialSpawnTimer++;
				if (initialSpawnTimer >= initialSpawnTimerNext)
				{
					initialSpawnTimer = 0;
					spawning = true;
					initialSpawnTimerNext = Main.rand.Next(initialSpawnTimerMin, initialSpawnTimerMax);
				}
			}
			if (spawning)
			{
				fastSpawnTimer++;
				if (fastSpawnTimer % fastSpawnRate == 0)
				{
					Dust dust = BitterRootDust.NewDust(player);
				}
				if (fastSpawnTimer >= fastSpawnTimerNext)
				{
					fastSpawnTimer = 0;
					fastSpawnTimerNext = fastSpawnRate * Main.rand.Next(fastSpawnTimerMin, fastSpawnTimerMax);
					spawning = false;
				}
			}
		}
	}
}
