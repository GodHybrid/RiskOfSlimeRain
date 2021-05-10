using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using System;
using Terraria;
using Terraria.ID;

namespace RiskOfSlimeRain.Core.ROREffects.Uncommon
{
	public class ArmsRace : RORUncommonEffect
	{
		public override float Initial => 0.09f;

		public override float Increase => 0.1f;

		public float mortarDamage => Stack * 1.7f;

		public override string Description => $"{Initial.ToPercent()} chance for minions to fire missiles and mortars";

		public override string FlavorText => "So, the upgrades for your drones are in.\nLet's continue this arms race, eh?";

		public override string UIInfo()
		{
			return $"Does nothing";
		}

	}
}
