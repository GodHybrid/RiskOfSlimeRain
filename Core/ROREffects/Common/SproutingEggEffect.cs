using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Dusts;
using RiskOfSlimeRain.Helpers;
using System;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Core.ROREffects.Common
{
	public class SproutingEggEffect : RORCommonEffect, IUpdateLifeRegen, IPlayerLayer
	{
		//const float Increase = 2.4f;
		private int TimerMax => ServerConfig.Instance.RorStats ? 420 : 600;

		public override float Initial => ServerConfig.Instance.RorStats ? 2.4f : 1.5f;

		public override float Increase => ServerConfig.Instance.RorStats ? 2.4f : 1.5f;

		public override string Description => $"Increases health regeneration by {Initial} health per second when out of combat for {TimerMax / 60} seconds";

		public override string FlavorText => "This egg seems to be somewhere between hatching and dying\nI can't bring it to myself to cook it alive";

		public override string UIInfo()
		{
			return $"Regeneration: {Formula()}/s";
		}

		public void UpdateLifeRegen(Player player)
		{
			if (player.GetRORPlayer().NoCombatTimer < TimerMax) return;

			//The number will be halved in redcode, hence the 2
			player.lifeRegen += (int)Math.Round(2 * Formula());

			if (Config.HiddenVisuals(player)) return;

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
			if (player.GetRORPlayer().NoCombatTimer > TimerMax)
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
