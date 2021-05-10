using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using System;
using Terraria;

namespace RiskOfSlimeRain.Core.ROREffects.Uncommon
{
	public class SmartShopperEffect : RORUncommonEffect
	{
		public override float Initial => ServerConfig.Instance.OriginalStats ? 0.25f : 0.1f;

		public override float Increase => ServerConfig.Instance.OriginalStats ? 0.25f : 0.1f;

		public override string Description => $"Killed enemies drop {Initial.ToPercent()} more money.";

		public override string FlavorText => "I swear the last $5 I used was the last one I had in my purse, but then when I looked again, I still had a bit left over.\nStrange, huh?";

		public override string UIInfo()
		{
			return $"Does nothing";
		}
	}
}
