using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using Terraria;

namespace RiskOfSlimeRain.Core.ROREffects.Uncommon
{
	public class HarvestersScytheEffect : RORUncommonEffect, IOnHit, IGetWeaponCrit
	{
		public const float initial = 6;
		public const float increase = 2;
		public const float critChance = 0.05f;

		public float CurrentHeal => initial + increase * Stack;

		//TODO
		public float StoredHeals = 0;

		public override string Description => $"Gain 5% crit chance. Critical strikes heal for {initial + increase} HP";
		public override string FlavorText => "It takes a brave man to look death in the eye and claim they don't need help.";
		public override string Name => "Harvester's Scythe";

		public void OnHitNPC(Player player, Item item, NPC target, int damage, float knockback, bool crit)
		{
			if (crit) player.HealMe((int)CurrentHeal);
		}

		public void OnHitNPCWithProj(Player player, Projectile proj, NPC target, int damage, float knockback, bool crit)
		{
			if (crit) player.HealMe((int)CurrentHeal);
		}

		public void GetWeaponCrit(Player player, Item item, ref int crit)
		{
			crit += (int)(critChance * 100);
		}
	}
}
