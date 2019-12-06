using RiskOfSlimeRain.Effects.Interfaces;
using System;
using Terraria;

namespace RiskOfSlimeRain.Effects.Common
{
	public class MysteriousVialEffect : ROREffect, IUpdateLifeRegen
	{
		const float increase = 1.2f;

		public override string Description => "Permanently increases health regeneration by 1.2 health per second";

		public override string FlavorText => "Side effects may include itching, rashes, bleeding, sensitivity of skin,\ndry patches, permanent scarring, misaligned bone regrowth, rotting of the...";

		public void UpdateLifeRegen(Player player)
		{
			//the number will be halved in redcode, hence the 2
			player.lifeRegen += (int)Math.Round(Stack * 2 * increase);
		}
	}
}
