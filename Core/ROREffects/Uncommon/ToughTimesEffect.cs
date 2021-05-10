using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using System;
using Terraria;

namespace RiskOfSlimeRain.Core.ROREffects.Uncommon
{
	public class ToughTimesEffect : RORUncommonEffect, IPostUpdateEquips
	{
		public override float Initial => 0.07f;

		public override float Increase => 0.07f;

		public override string Description => $"Increase damage reduction by {Initial.ToPercent()}, stacks multiplicatively";

		public override string FlavorText => "Bears are just about the only toy that can lose just about everything and still maintain their dignity and worth.\nDon't forget, hon. I'm coming home soon. Stay strong.";

		public override string UIInfo()
		{
			return $"Additional DR: {DRIncrease.ToPercent(2)}";
		}

		public override float Formula(bool stacksMultiplicatively = false)
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
