using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Dusts;
using RiskOfSlimeRain.Helpers;
using System;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Core.ROREffects.Uncommon
{
	public class RedWhipEffect : RORUncommonEffect, IPostUpdateRunSpeeds
	{
		//const float Increase = 2.4f;
		private float TimerMax => ServerConfig.Instance.OriginalStats ? 1.5f : 6f;
		private float SpeedBoost => ServerConfig.Instance.OriginalStats ? 0.8f : 0.6f;

		public override float Initial => ServerConfig.Instance.OriginalStats ? 0.5f : 2f;

		public override float Increase => ServerConfig.Instance.OriginalStats ? 1f : 4f;

		public override string Description => $"Leaving combat for {(TimerMax).ToString("n1")} seconds boosts your movement speed by {SpeedBoost.ToPercent()}.";

		public override string FlavorText => "I know exactly why you want these and I want you to never tell me.";

		public override bool EnforceMaxStack => true;
		public override int MaxRecommendedStack => 10;

		public override float Formula(bool stacksMultiplicatively = false)
		{
			return (float)(Initial + (Increase / Math.Max(Stack, 1)));
		}

		public override string UIInfo()
		{
			return $"Time required: {Formula().ToString("n1")}s";
		}

		public void PostUpdateRunSpeeds(Player player)
		{
			if (player.GetRORPlayer().NoCombatTimer < Formula()) return;

			player.maxRunSpeed += player.maxRunSpeed * SpeedBoost;
			player.moveSpeed += player.moveSpeed * SpeedBoost;
			player.accRunSpeed += 0.5f * SpeedBoost;
		}
	}
}
