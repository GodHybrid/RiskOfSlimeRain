using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.EntitySources;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Core.ROREffects.Common
{
	public class MonsterToothEffect : RORCommonEffect, IOnKill
	{
		//const int Initial = 5;
		//const int Increase = 5;

		public override float Initial => ServerConfig.Instance.OriginalStats ? 10f : 3f;

		public override float Increase => ServerConfig.Instance.OriginalStats ? 5f : 1f;

		public override LocalizedText Description => base.Description.WithFormatArgs(Initial);

		public override string UIInfo()
		{
			return UIInfoText.Format(Formula());
		}

		public void OnKillNPCWithItem(Player player, Item item, NPC target, NPC.HitInfo hit, int damageDone)
		{
			SpawnProjectile(player, target);
		}

		public void OnKillNPCWithProj(Player player, Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
		{
			SpawnProjectile(player, target);
		}

		void SpawnProjectile(Player player, NPC target)
		{
			if (NPCHelper.IsWormBodyOrTail(target)) return;
			if (NPCHelper.IsBossPiece(target)) return; //No free max health from creepers/probes/bees
			if (NPCHelper.IsSpawnedFromStatue(target)) return;
			if (target.type == NPCID.EaterofWorldsHead && !Main.rand.NextBool(10)) return;

			Projectile.NewProjectile(new EntitySource_FromEffect_Heal(player, this, (int)Formula()), target.Center, new Vector2(0f, -10f), ModContent.ProjectileType<PlayerHealthProj>(), 0, 0, Main.myPlayer);
		}
	}
}
