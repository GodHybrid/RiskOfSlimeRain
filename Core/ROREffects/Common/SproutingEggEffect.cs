using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core;
using RiskOfSlimeRain.Dusts;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using System;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Core.ROREffects.Common
{
	public class SproutingEggEffect : RORCommonEffect, IUpdateLifeRegen, IPlayerLayer
	{
		const float increase = 2.4f;
		const int timerMax = 420;

		public override string Description => $"Increases health regeneration by {increase} health per second when out of combat for {timerMax / 60} seconds";

		public override string FlavorText => "This egg seems to be somewhere between hatching and dying\nI can't bring it to myself to cook it alive";

		public void UpdateLifeRegen(Player player)
		{
			if (player.GetRORPlayer().NoCombatTimer < timerMax) return;

			//the number will be halved in redcode, hence the 2
			player.lifeRegen += (int)Math.Round(Stack * 2 * increase);

			if (Main.rand.NextBool(30))
			{
				Dust dust = Dust.NewDustDirect(player.position, player.width, player.height, ModContent.DustType<ColorableDustAlphaFade>(), 0, 0, 0, Color.Yellow * 0.78f, 1.4f);
				dust.customData = new InAndOutData(inSpeed: 5, outSpeed: 5, reduceScale: false);
				dust.velocity.X *= 0f;
				dust.velocity.Y = Main.rand.NextFloat(-0.7f, -0.2f);
			}
		}

		public PlayerLayerParams GetPlayerLayerParams(Player player)
		{
			if (player.GetRORPlayer().NoCombatTimer > timerMax)
			{
				return new PlayerLayerParams("Textures/SproutingEgg", new Vector2(-1f, -14f), Color.White * 0.5f);
			}
			else
			{
				return null;
			}
		}
	}
}
