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
	public class FrostRelicEffect : RORUncommonEffect, IOnKill
	{
		private float duration = 6.5f;

		public override float Initial => ServerConfig.Instance.OriginalStats ? 3f : 3f;

		public override float Increase => 1f;

		public override string Description => $"Killing an enemy surrounds you with {Initial} icicle{(Initial > 1 ? "s" : "")} that deal{(Initial == 1 ? "s" : "")} 3x33% damage.";

		public override string FlavorText => " Biggest snowflake, to date.\nTechnically, it isn't a snowflake, it's just a chunk of ice, but STILL.";

		public override string UIInfo()
		{
			return $"Does nothing";
		}

		public void OnKillNPC(Player player, Item item, NPC target, int damage, float knockback, bool crit)
		{
			throw new NotImplementedException();
		}

		public void OnKillNPCWithProj(Player player, Projectile proj, NPC target, int damage, float knockback, bool crit)
		{
			throw new NotImplementedException();
		}
	}
}
