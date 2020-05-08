using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Effects;
using RiskOfSlimeRain.Helpers;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WebmilioCommons.Tinq;

namespace RiskOfSlimeRain.Core.ROREffects.Common
{
	public class BarbedWireEffect : RORCommonEffect, IPostUpdateEquips, IPlayerLayer, IScreenShader
	{
		const int wireTimerMax = 60;
		const int initialWireRadius = 3;
		//const float Initial = 0.33f;
		//const float Increase = 0.17f;
		const int radIncrease = 16;

		int alphaCounter = 0;

		public override float Initial => 0.5f;

		public override float Increase => 0.17f;

		public float Alpha => (float)Math.Sin((alphaCounter / 6d) / (Math.PI * 2)) / 5f + 3 / 5f;

		int wireTimer = 0;

		int Radius => (initialWireRadius + Stack) * radIncrease;

		public override string Description => $"Touching enemies deals {Initial.ToPercent()} of your current damage every second";

		public override string FlavorText => "Disclaimer: I, or my company, am not responsible for any bodily harm delivered to...";

		public override string UIInfo()
		{
			return $"Current damage: {(int)Formula()}. Current radius: {Radius / radIncrease} Tiles";
		}

		public void PostUpdateEquips(Player player)
		{
			if (Main.hasFocus) alphaCounter++;
			if (Main.netMode == NetmodeID.Server || player.whoAmI != Main.myPlayer) return;
			wireTimer++;
			if (wireTimer > wireTimerMax)
			{
				NPC npc = Main.npc.FirstActiveOrDefault(n => n.CanBeChasedBy() && player.DistanceSQ(n.Center) <= (Radius + 16) * (Radius + 16));
				if (npc != null)
				{
					int damage = (int)(Formula() * player.GetDamage());
					player.ApplyDamageToNPC(npc, damage, 0f, 0, false);
					Item item = player.HeldItem;
					if (!item.IsAir)
					{
						ItemLoader.OnHitNPC(item, player, npc, damage, 0f, false);
						NPCLoader.OnHitByItem(npc, player, item, damage, 0f, false);
						PlayerHooks.OnHitNPC(player, item, npc, damage, 0f, false);
					}
				}
				wireTimer = 0;
			}
		}

		public PlayerLayerParams GetPlayerLayerParams(Player player)
		{
			return new PlayerLayerParams("Textures/BarbedWire", Vector2.Zero, Color.White * Alpha, scale: 1f);
		}

		public Effect GetScreenShader(Player player)
		{
			return ShaderManager.SetupCircleEffect(new Vector2((int)player.Center.X, (int)player.Center.Y + player.gfxOffY), Radius, Color.SandyBrown * (Alpha / 2) * ((255 - player.immuneAlpha) / 255f));
		}
	}
}
