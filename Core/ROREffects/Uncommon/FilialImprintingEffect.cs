using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using System;
using Terraria;

namespace RiskOfSlimeRain.Core.ROREffects.Uncommon
{
	public class FilialImprintingEffect : RORUncommonEffect
	{
		private float newBuffCooldown = 20f;
		public override float Initial => 1f;

		public override float Increase => 1f;

		public override string Description => $"Drops attack speed/regen/move speed buffs every {newBuffCooldown} seconds.";

		public override string FlavorText => "You didn't tell me the row was FERTILIZED. Good lord!\nAnyways, one of the suckers actually hatched, and have been nothing but friendly to me. Filial imprinting, perhaps.";

		public override string UIInfo()
		{
			return $"Does nothing";
		}
	}
}
