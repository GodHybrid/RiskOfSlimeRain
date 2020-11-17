using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using Terraria;

namespace RiskOfSlimeRain.Core.ROREffects.Uncommon
{
	//TODO needs rebalancing badly
	public class ToughTimesEffect : RORUncommonEffect, IPostUpdateEquips
	{
		public override float Initial => 14;

		public override float Increase => 7;

		public float DefInc => Formula();

		public override string Description => $"Increase armor by {Initial}";

		public override string FlavorText => "Bears are just about the only toy that can lose just about everything and still maintain their dignity and worth.\nDon't forget, hon. I'm coming home soon. Stay strong.";

		public void PostUpdateEquips(Player player)
		{
			player.statDefense += (int)DefInc;
		}
	}
}
