using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Effects.Interfaces;
using System;
using Terraria;

namespace RiskOfSlimeRain.Effects.Common
{
	public class SoldiersSyringeEffect : ROREffect, IUseTimeMultiplier
	{
		const float increase = 0.1f;

		public override int MaxStack => 13;

		public override string Description => "Increase attack speed by 15%";

		public override string FlavorText => "Should help multi-purpose requirements needed of soldiers\nContains vaccinations, antibiotics, pain killers, steroids, heroine, gasoline...";

		public void UseTimeMultiplier(Player player, Item item, ref float multiplier)
		{
			if (item.damage > 0 || item.axe > 0 || item.hammer > 0 || item.pick > 0) multiplier += Stack * increase; //15% is made into 10%, but it still works as 15%
		}
	}
}
