using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Dusts;
using RiskOfSlimeRain.Helpers;
using Terraria;

namespace RiskOfSlimeRain.Core.ROREffects.Common
{
	public class BitterRootEffect : RORCommonEffect, IResetEffects, IPostUpdateEquips
	{
		//const float Increase = 0.07895f;

		public const int initialSpawnTimerMin = 40;

		public const int initialSpawnTimerMax = 100;

		public const int fastSpawnTimerMin = 4;

		public const int fastSpawnTimerMax = 10;

		public const int fastSpawnRate = 3;
		
		public override float Initial => 0.07895f;

		public override float Increase => 0.07895f;

		public int initialSpawnTimer = 0;

		public int initialSpawnTimerNext = initialSpawnTimerMax;

		public int fastSpawnTimer = 0;

		public int fastSpawnTimerNext = fastSpawnTimerMax;

		public bool spawning = false;

		public override int MaxRecommendedStack => 38;

		public override bool EnforceMaxStack => true;

		public override string Description => $"Permanently increases maximum life by roughly {Increase.ToPercent(0)}";

		public override string FlavorText => "Biggest. Ginseng. Root. Ever.";

		public override string UIInfo()
		{
			return $"Life increase: {GetIncreaseAmount(Player)}";
		}

		public void ResetEffects(Player player)
		{
			player.statLifeMax2 += GetIncreaseAmount(player);
		}

		public int GetIncreaseAmount(Player player)
		{
			return (int)(player.statLifeMax * Formula());
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
					BitterRootDust.NewDust(player);
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
