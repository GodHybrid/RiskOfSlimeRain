using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Projectiles;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.World.Generation;

namespace RiskOfSlimeRain.Core.ROREffects.Uncommon
{
	public class UkuleleEffect : RORUncommonEffect
	{
		private float procChance = ServerConfig.Instance.OriginalStats ? 0.2f : 0.1f;
		public override float Initial => 0.33f;

		public override float Increase => 0.33f;

		public override string Description => $"{procChance.ToPercent()} chance to fire chain lighting for 4x33% damage";

		public override string FlavorText => "NOTE: No tests have managed to make the ukulele actually generate electricity.\nUse with care.";

		public override string UIInfo()
		{
			return $"Does nothing";
		}
	}
}
