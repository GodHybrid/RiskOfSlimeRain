using RiskOfSlimeRain.Effects.Interfaces;
using RiskOfSlimeRain.Helpers;
using Terraria;
using Terraria.ID;

namespace RiskOfSlimeRain.Effects.Common
{
	public class CrowbarEffect : ROREffect, IModifyHit
	{
		const float initial = 0.2f;
		const float increase = 0.3f;

		public override string Description => "Deal 50% more damage to enemies above 80% HP";

		public override string FlavorText => "Crowbar/prybar/wrecking bar allows for both prying and smashing! \nCarbon steel, so it should last for a very long time, at least until the 3rd edition arrives.";

		void ModifyDamage(NPC target, ref int damage)
		{
			SoundHelper.PlaySound(SoundID.Shatter, (int)target.Center.X, (int)target.Center.Y, -1, 1f, -0.7f);
			if (target.life >= target.lifeMax * 0.8f) damage += (int)(damage * (initial + increase * Stack));
		}

		public void ModifyHitNPC(Player player, Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
		{
			ModifyDamage(target, ref damage);
		}

		public void ModifyHitNPCWithProj(Player player, Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			ModifyDamage(target, ref damage);
		}
	}
}
