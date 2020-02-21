//using RiskOfSlimeRain.Core.ROREffects.Interfaces;
//using RiskOfSlimeRain.Helpers;
//using Terraria;

namespace RiskOfSlimeRain.Core.ROREffects.Uncommon
{
	//TODO Make it actually work lol
	class GuardiansHeartEffect : RORUncommonEffect
	{
		public const float initial = 60;
		public const float increase = 60;
		public const int time = 7;

		public float Shield => initial + increase * Stack;


		//public override bool AlwaysProc => true;
		public override string Description => $"Gain {initial} SP shields. Recharges in {time} seconds";
		public override string FlavorText => "While living, the subject had advanced muscle growth, cell regeneration, higher agility...\nHis heart seems to still beat independent of the rest of the body.";
		public override string Name => "Guardian's Heart";


	}
}
