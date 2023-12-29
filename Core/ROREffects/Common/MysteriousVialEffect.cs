using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Dusts;
using System;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Core.ROREffects.Common
{
	public class MysteriousVialEffect : RORCommonEffect, IUpdateLifeRegen
	{
		//const float Increase = 1.2f;

		public override float Initial => ServerConfig.Instance.OriginalStats ? 1.2f : 0.5f; //Band of Regeneration is 1f

		public override float Increase => ServerConfig.Instance.OriginalStats ? 1.2f : 0.5f;

		public override LocalizedText Description => base.Description.WithFormatArgs(Initial);

		public override string UIInfo()
		{
			return UIInfoText.Format(Formula());
		}

		public void UpdateLifeRegen(Player player)
		{
			//The number will be halved in redcode, hence the 2
			player.lifeRegen += (int)Math.Round(2 * Formula());

			if (Config.HiddenVisuals(player)) return;

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
