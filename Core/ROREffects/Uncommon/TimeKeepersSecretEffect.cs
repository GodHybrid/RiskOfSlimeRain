using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Dusts;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Projectiles;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Core.ROREffects.Uncommon
{
	public class TimeKeepersSecretEffect : RORUncommonEffect, IPostHurt, IPreHurt
	{
		private float timestopCooldown = 60 * 60 * 7f; //7 minutes to recharge after activation
		private float timestopActivationRange = 0.15f; //activates at 15% HP left
		public override float Initial => ServerConfig.Instance.OriginalStats ? 3f : 5f;

		public override float Increase => ServerConfig.Instance.OriginalStats ? 1f : 1f;

		public override bool EnforceMaxStack => true;
		public override int MaxRecommendedStack => ServerConfig.Instance.OriginalStats ? 8 : 8;

		public override string Description => $"Falling to low health stops time for {Initial} seconds.";

		public override string FlavorText => "My old grandfather's hourglass. As the Time Keeper in the Hall of The Revered, he valued this hourglass a lot.\n" +
			"For some reason, the sand never seems to run out!\nMy old grandfather's hourglass. As the Time Keeper in the Hall...";

		public override string Name => "Time Keeper's Secret";

		public override string UIInfo()
		{
			return $"Does nothing";
		}

		public void PostHurt(Player player, bool pvp, bool quiet, double damage, int hitDirection, bool crit)
		{
			throw new NotImplementedException();
		}

		public bool PreHurt(Player player, bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
		{
			throw new NotImplementedException();
		}
	}
}
