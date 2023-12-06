using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using System;
using Terraria;
using Terraria.Localization;

namespace RiskOfSlimeRain.Core.ROREffects.Uncommon
{
	public class ToughTimesEffect : RORUncommonEffect, IPostUpdateEquips
	{
		public override float Initial => 0.07f;

		public override float Increase => 0.07f;

		public override LocalizedText Description => base.Description.WithFormatArgs(Initial.ToPercent());

		public override string UIInfo()
		{
			return $"Additional DR: {DRIncrease.ToPercent(2)}";
		}

		public override float Formula()
		{
			if (Stack > 1)
			{
				return Initial + 1f - (float)Math.Pow(1f - Increase, Stack - 1);
			}
			else
			{
				return Initial;
			}
		}

		public float DRIncrease => Formula();

		public void PostUpdateEquips(Player player)
		{
			player.endurance += DRIncrease;
		}
	}
}
