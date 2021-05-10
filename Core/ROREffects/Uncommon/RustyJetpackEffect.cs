using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Dusts;
using RiskOfSlimeRain.Helpers;
using System;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Core.ROREffects.Uncommon
{
	public class RustyJetpackEffect : RORUncommonEffect, IPostUpdateEquips
	{
		private float descentSlowdown = 0.5f;

		public override float Initial => ServerConfig.Instance.OriginalStats ? 0.1f : 0.1f;

		public override float Increase => ServerConfig.Instance.OriginalStats ? 0.1f : 0.1f;

		public override string Description => $"Decrease gravity by {descentSlowdown.ToPercent()} and increase jump height by {(Initial).ToPercent()}";

		public override string FlavorText => "Sorry, it seems to be broken. It only works for a split second;\nMake sure to keep the kiddos away from the bottom; it shoots out quite a jet. Can make for fun obstacle challenges.";

		public override string UIInfo()
		{
			return $"Jump boost: {Formula().ToPercent()}";
		}

		public void PostUpdateEquips(Player player)
		{
			player.maxFallSpeed -= player.maxFallSpeed * descentSlowdown;
			//this is super wonky and funny especially with boots/wings, try it and break the game pls
			if (!player.releaseJump && player.oldPosition.Y > player.position.Y)
			{
				player.velocity = new Vector2(player.velocity.X, player.velocity.Y * (1 + Formula()));
			}
		}
	}
}
