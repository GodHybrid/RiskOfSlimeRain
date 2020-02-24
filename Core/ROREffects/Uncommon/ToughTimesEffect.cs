using RiskOfSlimeRain.Core.ROREffects.Interfaces;
//using RiskOfSlimeRain.Helpers;
using Terraria;

namespace RiskOfSlimeRain.Core.ROREffects.Uncommon
{
	public class ToughTimesEffect : RORUncommonEffect, IPostUpdateEquips
	{
		public const float initial = 0;
		public const float increase = 14;

		public float DefInc => initial + increase * Stack;

		public override string Description => $"Increase armor by {initial + increase}";
		public override string FlavorText => "Bears are just about the only toy that can lose just about everything and still maintain their dignity and worth.\nDon't forget, hon. I'm coming home soon. Stay strong.";

		public void PostUpdateEquips(Player player)
		{
			player.statDefense += (int)DefInc;
		}
	}
}
