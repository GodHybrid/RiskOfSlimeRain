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
		const float hplimit = 0.8f;

		public override string Description => $"Deal {(initial + increase).ToPercent()} more damage to enemies above {hplimit.ToPercent()} HP";

		public override string FlavorText => "Crowbar/prybar/wrecking bar allows for both prying and smashing! \nCarbon steel, so it should last for a very long time, at least until the 3rd edition arrives.";

		void ModifyDamage(NPC target, ref int damage)
		{
			SoundHelper.PlaySound(SoundID.Shatter, (int)target.Center.X, (int)target.Center.Y, -1, 0.8f, -0.7f);
			if (target.life >= target.lifeMax * hplimit) damage += (int)(damage * (initial + increase * Stack));
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
