using RiskOfSlimeRain.Core.ROREffects.Helpers;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using System;
using Terraria;

namespace RiskOfSlimeRain.Core.ROREffects.Uncommon
{
	public class LeechingSeedEffect : HealingPoolEffect, IOnHit, IPostUpdateEquips
	{
		public override RORRarity Rarity => RORRarity.Uncommon;

		public override int HitCheckMax => 5;

		public override float CurrentHeal => Formula();

		public override float Initial => ServerConfig.Instance.OriginalStats ? 0.2f : 0.1f;

		public override float Increase => ServerConfig.Instance.OriginalStats ? 0.1f : 0.05f;

		public override string Description => $"Dealing damage heals you for {Initial} health";

		public override string FlavorText => "These flesh-infesting pods seem to burrow, balloon, and then pop in a few months. Most test patients have... died.\nHowever, before they die, they feel increased health and state of mind!";

		public override string UIInfo()
		{
			return $"Stored heal: {Math.Round(StoredHeals, 2)}. Heal amount: {Math.Round(CurrentHeal, 2)}";
		}

		public void OnHitNPC(Player player, Item item, NPC target, int damage, float knockback, bool crit)
		{
			HandleAndApplyHeal(player);
		}

		public void OnHitNPCWithProj(Player player, Projectile proj, NPC target, int damage, float knockback, bool crit)
		{
			HandleAndApplyHeal(player);
		}

		public void PostUpdateEquips(Player player)
		{
			UpdateHitCheckCount(player);
		}
	}
}
