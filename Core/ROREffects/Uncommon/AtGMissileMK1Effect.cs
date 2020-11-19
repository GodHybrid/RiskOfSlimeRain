using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Projectiles;
using System;
using Terraria;
using Terraria.ID;

namespace RiskOfSlimeRain.Core.ROREffects.Uncommon
{
	public class AtGMissileMK1Effect : RORUncommonEffect, IOnHit, IPlayerLayer
	{
		public const float damageIncrease = 3f;

		private float DamageIncrease => ServerConfig.Instance.OriginalStats ? damageIncrease : damageIncrease - 1f;

		public override float Initial => 0.1f;

		public override int MaxRecommendedStack => 10;

		public override string Description => $"{Initial.ToPercent()} chance to fire a missile that deals {damageIncrease.ToPercent()} damage";

		public override string FlavorText => "Lightweight and attachable to torso for free use of both hands.\nCan store up to 120 heat-seaking missiles, which is just what your men need to fight off the [REDACTED]";

		public override string Name => "AtG Missile Mk. 1";

		public override string UIInfo()
		{
			return $"Spawn chance: {RollChance.ToPercent(2)}";
		}

		public PlayerLayerParams GetPlayerLayerParams(Player player)
		{
			return new PlayerLayerParams("Textures/AtGMissileMK1", Vector2.Zero);
		}

		public override float Formula()
		{
			if (Stack > 1)
			{
				return Initial + 1f - (float)Math.Pow(1f - Increase, Stack - 1);
			}
			else
			{
				return Initial;
			}
		}

		private float RollChance => Formula();

		public void OnHitNPC(Player player, Item item, NPC target, int damage, float knockback, bool crit)
		{
			RollSpawn(player);
		}

		public void OnHitNPCWithProj(Player player, Projectile proj, NPC target, int damage, float knockback, bool crit)
		{
			RollSpawn(player);
		}

		private void RollSpawn(Player player)
		{
			if (Proc(RollChance))
			{
				SpawnProjectile(player);
			}
			//for (int i = 0; i < Stack; i++)
			//{
			//	//Keep trying to roll Stack amount of times
			//	if (Proc(Chance))
			//	{
			//		SpawnProjectile(player);
			//		break;
			//	}
			//}
		}

		void SpawnProjectile(Player player)
		{
			//In ror, it actually uses the dealt damage for the missile damage, not the players damage
			int damage = (int)(DamageIncrease * player.GetDamage());
			if (Main.netMode != NetmodeID.Server) SoundHelper.PlaySound(SoundID.Item13.SoundId, (int)player.Center.X, (int)player.Center.Y, SoundID.Item13.Style, SoundHelper.FixVolume(2f), 0.4f);
			Vector2 velo = new Vector2(Main.rand.NextFloat(4f) - 2f, -2f);
			RandomMovementProj.NewProjectile<AtGMissileMK1Proj>(player.Center, velo, damage, 10f);
		}
	}
}
