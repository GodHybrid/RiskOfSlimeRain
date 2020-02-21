using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using Terraria;

namespace RiskOfSlimeRain.Core.ROREffects.Uncommon
{
	//TODO Makes mines drop
	class PanicMinesEffect : RORUncommonEffect, IPostHurt
	{
		public const float initial = 5;
		public const float increase = 1;
		public const float damage = 5f;

		public float MinesDropped => initial + increase * Stack;

		public override string Description => $"Drop mines at low health for {damage.ToPercent()} damage.";
		public override string FlavorText => "Must be strapped onto vehicles, NOT personnel!\nIncludes smart-fire, but leave the blast radius regardless. The laws of physics don't pick sides.";

		public void PostHurt(Player player, bool pvp, bool quiet, double damage, int hitDirection, bool crit)
		{
			throw new System.NotImplementedException();
		}
	}
}
