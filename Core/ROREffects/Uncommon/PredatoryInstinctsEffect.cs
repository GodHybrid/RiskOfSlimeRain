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
	public class PredatoryInstinctsEffect : RORUncommonEffect, IPostUpdateEquips, IOnHit, IPlayerLayer, IUseTimeMultiplier
	{
		public const float speedBoost = 0.05f;
		public const byte maxBoosts = 3;
		public byte currentBoost = 0;
		
		private int timerMax = 120;
		public override float Initial => 0.1f;

		public override float Increase => 0.01f;

		public float TotalSpeedIncrease => Formula();

		public override string Description => $"Critical strikes increase attack speed by {TotalSpeedIncrease.ToPercent()}. Stacks up to {(maxBoosts * TotalSpeedIncrease).ToPercent()}.";

		public override string FlavorText => "Wolves. Nasty creatures.\nThe hunter is now the hunted, eh? You ain't sayin much anymore, are ya?";

		public override string Name => "Predatory Instincts";

		public override string UIInfo()
		{
			return $"Attack speed boost cap: {(maxBoosts * TotalSpeedIncrease).ToPercent()}";
		}

		public void OnHitNPC(Player player, Item item, NPC target, int damage, float knockback, bool crit)
		{
			if (crit)
			{
				currentBoost = currentBoost < maxBoosts ? ++currentBoost : maxBoosts;
			}
		}

		public void OnHitNPCWithProj(Player player, Projectile proj, NPC target, int damage, float knockback, bool crit)
		{
			if (crit)
			{
				currentBoost = currentBoost < maxBoosts ? ++currentBoost : maxBoosts;
			}
		}

		public void PostUpdateEquips(Player player)
		{
			if (player.GetRORPlayer().NoCritTimer > timerMax) currentBoost = 0;
		}

		public PlayerLayerParams GetPlayerLayerParams(Player player)
		{
			if (currentBoost > 0) return new PlayerLayerParams("Textures/PredatoryInstincts", new Vector2(0, -46), frame: currentBoost - 1, frameCount: 3);
			else return null;
		}

		public void UseTimeMultiplier(Player player, Item item, ref float multiplier)
		{
			if (currentBoost > 0) 
				if (item.damage > 0 || item.axe > 0 || item.hammer > 0 || item.pick > 0) multiplier += TotalSpeedIncrease * currentBoost;
		}
	}
}
