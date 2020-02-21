using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using Terraria;

namespace RiskOfSlimeRain.Core.ROREffects.Uncommon
{
	//TODO Finish this lol kek
	class DeadMansFootEffect : RORUncommonEffect, IPostHurt
	{
		public const float initial = 3;
		public const float increase = 1;
		public const float damage = 1.5f;

		public float Ticks => initial + increase * Stack;


		//public override bool AlwaysProc => true;
		public override string Description => $"Drop a poison mine at low health for {initial + increase}x{damage.ToPercent()} damage.";
		public override string FlavorText => "It looks like he was infested by some bug-like creatures, and exploded when I got close.\nI hope his death wasn't too painful; his family will know how he died.";
		public override string Name => "Dead Man's Foot";

		public void PostHurt(Player player, bool pvp, bool quiet, double damage, int hitDirection, bool crit)
		{
			throw new System.NotImplementedException();
		}
	}
}
