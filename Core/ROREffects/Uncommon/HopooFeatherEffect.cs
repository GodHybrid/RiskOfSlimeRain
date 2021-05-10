using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using System;
using Terraria;

namespace RiskOfSlimeRain.Core.ROREffects.Uncommon
{
	public class HopooFeatherEffect : RORUncommonEffect
	{
		public override float Initial => 1f;

		public override float Increase => 1f;

		public override string Description => $"Gain another jump.";

		public override string FlavorText => "A feather from the hopoo, found only natively in Europa.\nThere are only 3 left; I hope you can keep their location safe.";

		public override string UIInfo()
		{
			return $"Does nothing";
		}
	}
}
