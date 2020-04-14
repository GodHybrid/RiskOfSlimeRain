using RiskOfSlimeRain.Core.NPCEffects;
using RiskOfSlimeRain.Core.NPCEffects.Common;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using Terraria;
using Terraria.ID;

namespace RiskOfSlimeRain.Core.ROREffects.Common
{
	public class TaserEffect : RORCommonEffect, IOnHit
	{
		//const int Initial = 10;
		//const int Increase = 5;
		
		public override float Initial => 15f;

		public override float Increase => 5f;

		public override string Description => $"{Chance.ToPercent()} chance to snare enemies for {Initial / 10f} seconds";

		public override string FlavorText => "You say you can fix 'em?\nThese tasers are very very faulty";

		public override bool AlwaysProc => false;

		public override float Chance => 0.07f;

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

			NPCEffectManager.ApplyNPCEffect<TaserNPCEffect>(target, (int)Formula() * 6, true, true);
		}
	}
}
