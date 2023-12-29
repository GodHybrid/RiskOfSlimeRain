using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Dusts;
using RiskOfSlimeRain.Helpers;
using System;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Core.ROREffects.Common
{
	public class SproutingEggEffect : RORCommonEffect, IUpdateLifeRegen
	{
		//const float Increase = 2.4f;
		private int TimerMax => ServerConfig.Instance.OriginalStats ? 420 : 600;

		public override float Initial => ServerConfig.Instance.OriginalStats ? 2.4f : 1.5f;

		public override float Increase => ServerConfig.Instance.OriginalStats ? 2.4f : 1.5f;

		public override LocalizedText Description => base.Description.WithFormatArgs(Initial, TimerMax / 60);

		public override string UIInfo()
		{
			return UIInfoText.Format(Formula());
		}

		public void UpdateLifeRegen(Player player)
		{
			if (player.GetRORPlayer().NoCombatTimer < TimerMax) return;

			//The number will be halved in redcode, hence the 2
			player.lifeRegen += (int)Math.Round(2 * Formula());

			if (Config.HiddenVisuals(player)) return;

			if (Main.rand.NextBool(28))
			{
				Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, ModContent.DustType<ColorableDustAlphaFade>(), 0, 0, 0, Color.Yellow * 0.78f, 1.4f);
				dust.customData = new InAndOutData(inSpeed: 5, outSpeed: 5, reduceScale: false);
				dust.velocity.X *= 0f;
				dust.velocity.Y = Main.rand.NextFloat(-0.7f, -0.2f);
			}
		}
	}
}
