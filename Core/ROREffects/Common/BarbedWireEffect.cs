using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Effects;
using RiskOfSlimeRain.Helpers;
using System;
using Terraria;
using WebmilioCommons.Tinq;

namespace RiskOfSlimeRain.Core.ROREffects.Common
{
	public class BarbedWireEffect : RORCommonEffect, IPostUpdateEquips, IPlayerLayer, IScreenShader
	{
		const int wireTimerMax = 60;
		const int wireRadius = 2;
		const float initial = 0.33f;
		const float increase = 0.17f;
		const int radIncrease = 16;

		int alphaCounter = 0;

		public float Alpha => (float)Math.Sin((alphaCounter / 6d) / (Math.PI * 2)) / 5f + 3 / 5f;

		int wireTimer = 0;
		int Radius => (wireRadius + Stack) * radIncrease;

		public override string Description => $"Touching enemies deals {(initial + increase).ToPercent()} of your current damage every second";

		public override string FlavorText => "Disclaimer: I, or my company, am not responsible for any bodily harm delivered to...";

		public void PostUpdateEquips(Player player)
		{
			if (Main.hasFocus) alphaCounter++;
			if (Main.myPlayer != player.whoAmI) return;
			wireTimer++;
			if (wireTimer > wireTimerMax)
			{
				NPC npc = Main.npc.FirstActiveOrDefault(n => n.CanBeChasedBy() && player.DistanceSQ(n.Center) <= (Radius + 16) * (Radius + 16));
				if (npc != null)
				{
					player.ApplyDamageToNPC(npc, (int)((initial + increase * Stack) * player.GetDamage()), 0f, 0, false);
				}
				wireTimer = 0;
			}
		}

		public PlayerLayerParams GetPlayerLayerParams(Player player)
		{
			return new PlayerLayerParams("Textures/BarbedWire", Vector2.Zero, Color.White * Alpha, scale: 3f);
		}

		public Effect GetScreenShader(Player player)
		{
			return ShaderManager.SetupCircleEffect(new Vector2((int)player.Center.X, (int)player.Center.Y + player.gfxOffY), Radius, Color.SandyBrown * (Alpha / 2) * ((255 - player.immuneAlpha) / 255f));
		}
	}
}
