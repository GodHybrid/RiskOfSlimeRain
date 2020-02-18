using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using Terraria;

namespace RiskOfSlimeRain.Core.ROREffects.Common
{
	/// <summary>
	/// Effects of this on the player are hardcoded in RORInterfaceLayers for now
	/// </summary>
	public class SoldiersSyringeEffect : RORCommonEffect, IUseTimeMultiplier, IPostUpdateEquips
	{
		const float increase = 0.1f;
		public const int shakeTimerMax = 6; //to and back

		public Vector2 shakePosOffset = default(Vector2);

		public Vector2 shakeScaleOffset = default(Vector2);

		public int shakeTimer = 0;

		public bool increment = true;

		public override string Name => "Soldier's Syringe";
		public override int MaxRecommendedStack => 13;

		public override string Description => $"Increase attack speed by {(increase + 0.05f).ToPercent()}";

		public override string FlavorText => "Should help multi-purpose requirements needed of soldiers\nContains vaccinations, antibiotics, pain killers, steroids, heroine, gasoline...";

		public void UseTimeMultiplier(Player player, Item item, ref float multiplier)
		{
			if (item.damage > 0 || item.axe > 0 || item.hammer > 0 || item.pick > 0) multiplier += Stack * increase; //15% is made into 10%, but it still works as 15%
		}

		public void PostUpdateEquips(Player player)
		{
			if (increment)
			{
				if (shakeTimer < shakeTimerMax)
				{
					shakeTimer++;
					if (shakeTimer >= shakeTimerMax) increment = false;
				}
			}
			else
			{
				if (shakeTimer > 0)
				{
					shakeTimer--;
					if (shakeTimer <= 0)
					{
						shakePosOffset = new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f)) * 3 / shakeTimerMax;
						shakeScaleOffset = new Vector2(Main.rand.NextFloat(-0.1f, 0.1f), Main.rand.NextFloat(-0.1f, 0.1f)) * 3 / shakeTimerMax;
						increment = true;
					}
				}
			}
		}
	}
}
