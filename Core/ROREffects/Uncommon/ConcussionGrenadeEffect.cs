using RiskOfSlimeRain.Core.NPCEffects;
using RiskOfSlimeRain.Core.NPCEffects.Uncommon;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using Terraria;
using Terraria.ID;

namespace RiskOfSlimeRain.Core.ROREffects.Uncommon
{
	public class ConcussionGrenadeEffect : RORUncommonEffect, IOnHit
	{
		private int duration = 2;

		public override float Initial => 0f;

		public override float Increase => 0.06f;

		public override string Description => $"{(Initial + Increase).ToPercent()} chance to stun enemies for {duration} seconds";

		public override string FlavorText => "Pull the pin and throw it in the general direction of the bad guys.\nIf you can't figure it out, you deserve to get hurt.";

		public override string UIInfo()
		{
			return $"Chance to stun: {Chance.ToPercent(2)}";
		}

		public override bool AlwaysProc => false;

		public override float Chance => Formula(stacksMultiplicatively: true);

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
			if (target.boss && !Main.rand.NextBool(10)) return;
			if (NPCHelper.IsWormBodyOrTail(target)) return;
			if (target.type == NPCID.WallofFlesh || target.type == NPCID.WallofFleshEye) return;
			if (NPCHelper.IsBossPiece(target)) return;

			NPCEffectManager.ApplyNPCEffect<ConcussionNPCEffect>(target, 60 * duration, true, true);
		}
	}
}
