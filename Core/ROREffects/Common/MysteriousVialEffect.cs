using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Dusts;
using System;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Core.ROREffects.Common
{
	public class MysteriousVialEffect : RORCommonEffect, IUpdateLifeRegen
	{
		//const float Increase = 1.2f;

		public override float Initial => 1.2f;

		public override float Increase => 1.2f;

		public override string Description => $"Permanently increases health regeneration by {Increase} health per second";

		public override string FlavorText => "Side effects may include itching, rashes, bleeding, sensitivity of skin,\ndry patches, permanent scarring, misaligned bone regrowth, rotting of the...";

		public void UpdateLifeRegen(Player player)
		{
			if (Config.HiddenVisuals(player)) return;
			//the number will be halved in redcode, hence the 2
			player.lifeRegen += (int)Math.Round(2 * Formula());

			if (Main.rand.NextBool(30))
			{
				Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, ModContent.DustType<ColorableDustAlphaFade>(), 0, 0, 0, Color.LightGreen * 0.9f);
				dust.customData = new InAndOutData(inSpeed: 10, outSpeed: 10, reduceScale: false);
				dust.velocity.X *= 0f;
				dust.velocity.Y = Main.rand.NextFloat(-1f, -0.5f);
			}
		}
	}
}
