using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Dusts;
using RiskOfSlimeRain.Effects.Interfaces;
using System;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Effects.Common
{
	public class MysteriousVialEffect : RORCommonEffect, IUpdateLifeRegen
	{
		const float increase = 1.2f;

		public override string Description => $"Permanently increases health regeneration by {increase} health per second";

		public override string FlavorText => "Side effects may include itching, rashes, bleeding, sensitivity of skin,\ndry patches, permanent scarring, misaligned bone regrowth, rotting of the...";

		public void UpdateLifeRegen(Player player)
		{
			//the number will be halved in redcode, hence the 2
			player.lifeRegen += (int)Math.Round(Stack * 2 * increase);

			if (Main.rand.NextBool(30))
			{
				Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, ModContent.DustType<ColorableDustAlphaFade>(), 0, 0, 0, Color.LightGreen * 0.9f);
				//TODO - customData 10
				//dust.customData = 10;
				dust.customData = new InAndOutData(reduceScale: false);
				dust.velocity.X *= 0f;
				dust.velocity.Y = Main.rand.NextFloat(-2f, -1.5f);
			}
		}
	}
}
