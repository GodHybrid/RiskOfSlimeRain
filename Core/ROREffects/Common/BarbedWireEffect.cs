using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Effects;
using RiskOfSlimeRain.Helpers;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

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

		int Radius => (initialWireRadius + Math.Max(1, Stack)) * radIncrease;
		int RadiusSQ => (Radius + 16) * (Radius + 16);

		public override LocalizedText Description => base.Description.WithFormatArgs(Initial.ToPercent());

		public override string UIInfo()
		{
			return UIInfoText.Format(Formula().ToPercent(), Radius / radIncrease);
		}

		public void PostUpdateEquips(Player player)
		{
			if (Main.hasFocus) alphaCounter++;
			if (Main.netMode == NetmodeID.Server || player.whoAmI != Main.myPlayer) return;

			if (wireTimer >= wireTimerMax)
			{
				for (int i = 0; i < Main.maxNPCs; i++)
				{
					NPC npc = Main.npc[i];

					if (npc.CanBeChasedBy() && player.DistanceSQ(npc.Center) <= RadiusSQ)
					{
						int damage = (int)(Formula() * player.GetDamage());
						player.ApplyDamageToNPC_ProcHeldItem(npc, damage, damageType: ModContent.GetInstance<ArmorPenDamageClass>());
						wireTimer = 0;
						break;
					}
				}
			}
			else
			{
				wireTimer++;
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
