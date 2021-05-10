using RiskOfSlimeRain.Core.NPCEffects;
using RiskOfSlimeRain.Core.NPCEffects.Uncommon;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using Terraria;
using Terraria.ID;

namespace RiskOfSlimeRain.Core.ROREffects.Uncommon
{
	public class PrisonShacklesEffect : RORUncommonEffect, IOnHit
	{
		private float slowdown = 0.2f;

		public override float Initial => 1.5f;

		public override float Increase => 0.5f;

		public float totalDuration => Formula();

		public override string Description => $"For {Initial + Increase} seconds slow enemies by {slowdown.ToPercent()} on every attack";

		public override string FlavorText => "An artifact from old ages, before we used holo-cuffs.\nThe advantage is that EMP blasts and remote-hacking can't disable good-old steel and chains";

		public override string UIInfo()
		{
			return $"Slow duration: {totalDuration}";
		}

		public override bool AlwaysProc => true;

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

			NPCEffectManager.ApplyNPCEffect<ShacklesSlowdownNPCEffect>(target, (int)(60 * totalDuration), true, true);
		}
	}
}
