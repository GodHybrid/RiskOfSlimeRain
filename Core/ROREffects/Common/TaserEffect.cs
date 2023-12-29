using RiskOfSlimeRain.Core.NPCEffects;
using RiskOfSlimeRain.Core.NPCEffects.Common;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace RiskOfSlimeRain.Core.ROREffects.Common
{
	public class TaserEffect : RORCommonEffect, IOnHit
	{
		//const int Initial = 10;
		//const int Increase = 5;

		public override float Initial => 1.5f;

		public override float Increase => 0.5f;

		public override LocalizedText Description => base.Description.WithFormatArgs(Chance.ToPercent(), Initial);

		public override string UIInfo()
		{
			return UIInfoText.Format(Formula());
		}

		public override bool AlwaysProc => false;

		public override float Chance => ServerConfig.Instance.OriginalStats ? 0.07f : 0.1f;

		public void OnHitNPCWithItem(Player player, Item item, NPC target, NPC.HitInfo hit, int damageDone)
		{
			AddBuff(target);
		}

		public void OnHitNPCWithProj(Player player, Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
		{
			AddBuff(target);
		}

		void AddBuff(NPC target)
		{
			if (target.boss && !Main.rand.NextBool(10)) return;
			if (NPCHelper.IsWormBodyOrTail(target)) return;
			if (target.type == NPCID.WallofFlesh || target.type == NPCID.WallofFleshEye) return;
			if (NPCHelper.IsBossPiece(target)) return;

			NPCEffectManager.ApplyNPCEffect<TaserNPCEffect>(target, (int)Formula() * 60, true, true);
		}
	}
}
