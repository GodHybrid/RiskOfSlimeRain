using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Effects;
using RiskOfSlimeRain.Helpers;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WebmilioCommons.Tinq;

namespace RiskOfSlimeRain.Core.ROREffects.Uncommon
{
	public class ChargefieldGeneratorEffect : RORUncommonEffect, IPostUpdateEquips, IOnKill, IScreenShader
	{
		const int generatorHurtCD = 60;
		const int generatorTimerMax = 360;
		const int initialGeneratorRadius = 8;
		const int radIncrease = 2;
		public int radiusIncreaseStage = 0;

		public override float Initial => 1f;

		public override float Increase => 0.1f;

		private int generatorHurtTimer = 0;
		private int generatorTimeout = 0;
		private int alphaCounter = 0;
		private String maxLabel = "(MAX)";

		public float Alpha => (float)Math.Sin((alphaCounter / 6d) / (Math.PI * 2)) / 5f + 3 / 5f;

		int Radius => (initialGeneratorRadius + Math.Max(1, Stack)) * radIncrease * radiusIncreaseStage;

		public override string Description => $"Killing an enemy generates a lightning ring, dealing {(Initial + Increase).ToPercent()} damage/sec for {generatorTimerMax / 60} seconds";

		public override string FlavorText => "You know those anti-theft devices you can stick on your car?  Yeah, it's like that.\nYou need to charge it generally after every use.";

		public override string UIInfo()
		{
			return $"Current damage: {Formula().ToPercent()}. Current radius: {Radius / radIncrease}{(radiusIncreaseStage >= 30 ? maxLabel : "")} Tiles";
		}

		public void PostUpdateEquips(Player player)
		{
			if (Main.hasFocus) alphaCounter++;
			if (radiusIncreaseStage > 0)
			{ 
				if (Main.netMode == NetmodeID.Server || player.whoAmI != Main.myPlayer) return;
				generatorHurtTimer++;
				if (generatorHurtTimer > generatorHurtCD)
				{
					List<NPC> npcInRadius = Main.npc.WhereActive(n => n.CanBeChasedBy() && player.DistanceSQ(n.Center) <= (Radius + 16) * (Radius + 16));
					if (npcInRadius.Count > 0)
					{
						int damage = (int)(Formula() * player.GetDamage() * Math.Max(0.3f, (2f / (Math.Sqrt(npcInRadius.Count) + 1))));
						Item item = player.HeldItem;
						foreach (NPC npcIndexed in npcInRadius)
						{
							player.ApplyDamageToNPC(npcIndexed, damage, 0f, 0, false);
							if (!item.IsAir)
							{
								ItemLoader.OnHitNPC(item, player, npcIndexed, damage, 0f, false);
								NPCLoader.OnHitByItem(npcIndexed, player, item, damage, 0f, false);
								PlayerHooks.OnHitNPC(player, item, npcIndexed, damage, 0f, false);
							}
						}
					}
					generatorHurtTimer = 0;
				}
				generatorTimeout--;
				if (generatorTimeout <= 0)
				{
					  radiusIncreaseStage = 0;
				}
			}
		}

		void IOnKill.OnKillNPC(Player player, Item item, NPC target, int damage, float knockback, bool crit)
		{
			generatorTimeout = generatorTimerMax;
			IncreaseRadiusStage();
		}

		void IOnKill.OnKillNPCWithProj(Player player, Projectile proj, NPC target, int damage, float knockback, bool crit)
		{
			generatorTimeout = generatorTimerMax;
			IncreaseRadiusStage();
		}

		Effect IScreenShader.GetScreenShader(Player player)
		{
			return ShaderManager.SetupCircleEffect(new Vector2((int)player.Center.X, (int)player.Center.Y + player.gfxOffY), Radius, Color.AliceBlue * (Alpha / 2) * ((255 - player.immuneAlpha) / 255f));
		}

		public void IncreaseRadiusStage()
		{
			radiusIncreaseStage = Math.Min(++radiusIncreaseStage, 30);
		}
	}
}
