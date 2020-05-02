using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Projectiles;
using Terraria;
using Terraria.ID;

namespace RiskOfSlimeRain.Core.ROREffects.Common
{
	public class MonsterToothEffect : RORCommonEffect, IOnKill
	{
		//const int Initial = 5;
		//const int Increase = 5;

		public override float Initial => ServerConfig.Instance.RorStats ? 10f : 3f;

		public override float Increase => ServerConfig.Instance.RorStats ? 5f : 1f;

		public override string Description => $"Killing an enemy will heal you for {Initial} health";

		public override string FlavorText => "Sometimes I felt like it helped me on hunts, ya know?\nLike... instincts";

		public override string UIInfo()
		{
			return $"Heal amount: {Formula()}";
		}

		public void OnKillNPC(Player player, Item item, NPC target, int damage, float knockback, bool crit)
		{
			SpawnProjectile(target);
		}

		public void OnKillNPCWithProj(Player player, Projectile proj, NPC target, int damage, float knockback, bool crit)
		{
			SpawnProjectile(target);
		}

		void SpawnProjectile(NPC target)
		{
			if (NPCHelper.IsWormBodyOrTail(target)) return;
			if (NPCHelper.IsBossPiece(target)) return; //No free max health from creepers/probes/bees
			if (NPCHelper.IsSpawnedFromStatue(target)) return;
			if (target.type == NPCID.EaterofWorldsHead && !Main.rand.NextBool(10)) return;

			PlayerBonusProj.NewProjectile(target.Center, new Vector2(0f, -10f), onCreate: delegate (PlayerHealthProj proj)
			{
				proj.HealAmount = (int)Formula();
			});
		}
	}
}
