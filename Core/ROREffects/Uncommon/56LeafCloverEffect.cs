using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using System;
using Terraria;
using Terraria.ID;

namespace RiskOfSlimeRain.Core.ROREffects.Uncommon
{
	public class _56LeafClover : RORUncommonEffect
	{
		public override float Initial => 0.04f;

		public override float Increase => 0.015f;

		public override string Description => $"Elite enemies have a {Initial.ToPercent()} chance to drop items";

		public override string FlavorText => "A FIFTY-FREAKIN-SIX leaf clover! It lived through the mountain fire; imagine that!\nI'm keeping it in observation to make sure it doesn't wilt; this is a once in a lifetime specimen!";

		public override string UIInfo()
		{
			return $"Does nothing";
		}
	}
}
