using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RiskOfSlimeRain.Effects.Interfaces;
using RiskOfSlimeRain.Effects.Shaders;
using RiskOfSlimeRain.Helpers;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Effects.Common
{
	public class BarbedWireEffect : ROREffect, IPostUpdateEquips, IModifyDrawLayers
	{
		const int wireTimerMax = 60;
		const int wireRadius = 2;
		const float initial = 0.33f;
		const float increase = 0.17f;
		const int radIncrease = 16;

		int wireTimer = 0;
		int Radius => (wireRadius + Stack) * radIncrease;

		public override string Description => $"Touching enemies deals {(initial + increase).ToPercent()} of your current damage every second";

		public override string FlavorText => "Disclaimer: I, or my company, am not responsible for any bodily harm delivered to...";

		public void PostUpdateEquips(Player player)
		{
			if (Main.myPlayer != player.whoAmI) return;
			wireTimer++;
			if (wireTimer > wireTimerMax)
			{
				for (int m = 0; m < Main.maxNPCs; m++)
				{
					NPC enemy = Main.npc[m];
					if (enemy.CanBeChasedBy() && player.DistanceSQ(enemy.Center) <= (Radius + 16) * (Radius + 16))
					{
						player.ApplyDamageToNPC(enemy, (int)((initial + increase * Stack) * player.GetDamage()), 0f, 0, false);
					}
				}
				wireTimer = 0;
			}
		}

		public static readonly PlayerLayer MiscEffectsBack = new PlayerLayer("RiskOfSlimeRain", "ItemEffects", PlayerLayer.MiscEffectsBack, delegate (PlayerDrawInfo drawInfo)
		{
			if (drawInfo.shadow != 0f)
			{
				return;
			}
			Player drawPlayer = drawInfo.drawPlayer;

			float scale = 3f;
			Texture2D tex = ModContent.GetTexture("RiskOfSlimeRain/Textures/BarbedWire");
			int drawX = (int)(drawPlayer.Center.X - tex.Width * 0.5f * scale - Main.screenPosition.X);
			int drawY = (int)(drawPlayer.Center.Y + (int)drawPlayer.gfxOffY - tex.Width * 0.5f * scale - Main.screenPosition.Y);
			DrawData data = new DrawData(tex, new Vector2(drawX, drawY), null, Color.White * (0.6f * (255 - drawPlayer.immuneAlpha) / 255f), 0, new Vector2(0, 0), scale, SpriteEffects.None, 0);
			Main.playerDrawData.Add(data);

			RORPlayer mPlayer = drawPlayer.GetModPlayer<RORPlayer>();
			BarbedWireEffect bEffect = ROREffectManager.GetEffectOfType<BarbedWireEffect>(mPlayer);

			if (bEffect == null) return;

			Effect circle = ShaderManager.SetupCircleEffect(drawPlayer.Center + new Vector2(0f, drawPlayer.gfxOffY), bEffect.Radius, Color.SandyBrown * 0.5f);
			if (circle != null)
			{
				ShaderManager.ApplyToScreen(Main.spriteBatch, circle);
			}


			//scale = modPlayer.wireRadius * modPlayer.barbedWires;
			//drawX = (int)(drawPlayer.Center.X - tex.Width * 0.5f * scale - Main.screenPosition.X);
			//drawY = (int)(drawPlayer.Center.Y - tex.Width * 0.5f * scale - Main.screenPosition.Y);
			//data = new DrawData(tex, new Vector2(drawX, drawY), null, Color.White * 0.2f, 0, new Vector2(0, 0), scale, SpriteEffects.None, 0);
			//Main.playerDrawData.Add(data);
		});

		public void ModifyDrawLayers(List<PlayerLayer> layers)
		{
			layers.Insert(0, MiscEffectsBack);
		}
	}
}
