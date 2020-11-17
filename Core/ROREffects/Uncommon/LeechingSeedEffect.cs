using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using Terraria;

namespace RiskOfSlimeRain.Core.ROREffects.Uncommon
{
	public class LeechingSeedEffect : RORUncommonEffect, IOnHit
	{
		public const float initial = 1;
		public const float increase = 1;

		public float CurrentHeal => initial + increase * Stack;

		public float StoredHeals = 0;

		//public override bool AlwaysProc => true;
		public override string Description => $"Dealing damage heals you for {initial + increase} health";
		public override string FlavorText => "These flesh-infesting pods seem to burrow, balloon, and then pop in a few months. Most test patients have... died.\nHowever, before they die, they feel increased health and state of mind!";

		public void OnHitNPC(Player player, Item item, NPC target, int damage, float knockback, bool crit)
		{
			if (target.CanBeChasedBy()) player.HealMe((int)CurrentHeal);
		}

		public void OnHitNPCWithProj(Player player, Projectile proj, NPC target, int damage, float knockback, bool crit)
		{
			if (target.CanBeChasedBy()) player.HealMe((int)CurrentHeal);
		}
	}
}
