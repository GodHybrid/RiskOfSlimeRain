using RiskOfSlimeRain.Buffs;
using RiskOfSlimeRain.Effects.Interfaces;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Effects.Common
{
	public class RustyKnifeEffect : ROREffect, IOnHit
	{
		public override int MaxStack => 7;

		public override string Description => "15% chance to cause bleeding";

		public override string FlavorText => "Murder weapon, case name ELIAS. Probably a lover's spat?\nThere is still dried blood on the knife, so mark it as biological.";

		public override float Chance => Stack * 0.15f;

		public void OnHitNPC(Player player, Item item, NPC target, int damage, float knockback, bool crit)
		{
			AddBuff(target);
		}

		public void OnHitNPCWithProj(Player player, Projectile proj, NPC target, int damage, float knockback, bool crit)
		{
			AddBuff(target);
		}

		void AddBuff(NPC target)
		{
			target.AddBuff(ModContent.BuffType<KnifeBleed>(), 120);
		}
	}
}
