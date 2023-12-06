using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Projectiles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;

namespace RiskOfSlimeRain.Core.ROREffects.Uncommon
{
	public class AtGMissileMK1Effect : RORUncommonEffect, IOnHit, IPlayerLayer, IPostUpdateEquips
	{
		public const float damageIncrease = 3f;

		private float DamageIncrease => ServerConfig.Instance.OriginalStats ? damageIncrease : damageIncrease - 1f;

		public override float Initial => 0.1f;

		public override float Increase => 0.1f;
		
		int alphaCounter = 0;

		public float Alpha => (float)Math.Sin((alphaCounter / 6d) / (Math.PI * 2)) / 4f + 3 / 4f;

		public override LocalizedText Description => base.Description.WithFormatArgs(Initial.ToPercent(), damageIncrease.ToPercent());

		public override string UIInfo()
		{
			return $"Spawn chance: {RollChance.ToPercent(2)}";
		}

		public PlayerLayerParams GetPlayerLayerParams(Player player)
		{
			return new PlayerLayerParams("Textures/AtGMissileMK1", Vector2.Zero, Color.White * Alpha);
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

		public void OnHitNPC(Player player, Item item, NPC target, NPC.HitInfo hit, int damageDone)
		{
			RollSpawn(player);
		}

		public void OnHitNPCWithProj(Player player, Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
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
			if (Main.netMode != NetmodeID.Server) SoundEngine.PlaySound(SoundID.Item13.WithVolumeScale(SoundHelper.FixVolume(2f)).WithPitchOffset(0.4f), player.Center);
			Vector2 velo = new Vector2(Main.rand.NextFloat(4f) - 2f, -2f);
			RandomMovementProj.NewProjectile<AtGMissileMK1Proj>(GetEntitySource(player), player.Center, velo, damage, 10f);
		}

		public void PostUpdateEquips(Player player)
		{
			if (Main.hasFocus) alphaCounter++;
		}
	}
}
