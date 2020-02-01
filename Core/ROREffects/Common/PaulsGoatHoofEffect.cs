﻿using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using System;
using Terraria;

namespace RiskOfSlimeRain.Core.ROREffects.Common
{
	public class PaulsGoatHoofEffect : RORCommonEffect, IPostUpdateRunSpeeds
	{
		const float increase = 0.2f;

		public override string Name => "Paul's Goat Hoof";

		public override string Description => $"Run {increase.ToPercent()} faster";

		public override string FlavorText => "A hoof from one of my many goats\nThinking it was cancerous, I went to the doctors and low-and-behold; it was";

		public void PostUpdateRunSpeeds(Player player)
		{
			player.maxRunSpeed += player.maxRunSpeed * increase * Stack;
			player.moveSpeed += player.moveSpeed * increase * Stack;
			if (((player.controlRight && player.velocity.X < -9) || (player.controlLeft && player.velocity.X > 9d)) && Stack > 5) player.velocity.X /= 1.3f;

			if (Math.Abs(player.velocity.X) > 5f)
			{
				//TODO visuals (those lines)
			}
		}
	}
}