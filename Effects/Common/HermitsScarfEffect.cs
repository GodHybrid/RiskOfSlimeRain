using RiskOfSlimeRain.Effects.Interfaces;
using Terraria;
using Terraria.DataStructures;

namespace RiskOfSlimeRain.Effects.Common
{
	public class HermitsScarfEffect : ROREffect, IPreHurt
	{
		const float initial = 0.05f;
		const float increase = 0.05f;

		public override int MaxStack => 6;

		public override string Name => "Hermit's Scarf";

		public override string Description => "Allows you to evade attacks with 10% chance";

		public override string FlavorText => "This thing survived that horrible day\nIt must be able to survive whatever I have to endure, right?";

		public override float Chance => initial + increase * Stack;

		public bool PreHurt(Player player, bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
		{
			player.immune = true;
			player.immuneTime = 40;
			return false;
		}
	}
}
