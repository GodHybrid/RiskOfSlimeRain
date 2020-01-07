using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RiskOfSlimeRain.Dusts;
using RiskOfSlimeRain.Effects.Interfaces;
using RiskOfSlimeRain.Helpers;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Effects.Common
{
	public class SproutingEggEffect : ROREffect, IUpdateLifeRegen, IModifyDrawLayers
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

		public void ModifyDrawLayers(Player player, List<PlayerLayer> layers)
		{
			if (player.GetRORPlayer().NoCombatTimer > timerMax) layers.Insert(0, SproutingEggLayer);
		}

		public static readonly PlayerLayer SproutingEggLayer = new PlayerLayer("RiskOfSlimeRain", "SproutingEgg", PlayerLayer.MiscEffectsBack, delegate (PlayerDrawInfo drawInfo)
		{
			if (drawInfo.shadow != 0f)
			{
				return;
			}
			Player player = drawInfo.drawPlayer;

			Texture2D tex = ModContent.GetTexture("RiskOfSlimeRain/Textures/SproutingEgg");
			float drawX = (int)player.Center.X - Main.screenPosition.X;
			float drawY = (int)player.Center.Y + player.gfxOffY - Main.screenPosition.Y;

			drawY -= 12f;
			DrawData data = new DrawData(tex, new Vector2(drawX, drawY), null, Color.White * 0.5f * ((255 - player.immuneAlpha) / 255f), 0, tex.Size() / 2, 1f, SpriteEffects.None, 0);
			Main.playerDrawData.Add(data);
		});
	}
}
