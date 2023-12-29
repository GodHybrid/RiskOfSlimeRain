using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Projectiles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;

namespace RiskOfSlimeRain.Core.ROREffects.Common
{
	public class BundleOfFireworksEffect : RORCommonEffect, IOnKill
	{
		//const float Initial = 0.005f;
		//const float Increase = 0.005f;
		public const int fireworkCount = 5;
		public uint killCount = 0;

		private float DamageIncrease => ServerConfig.Instance.OriginalStats ? 9f : 7f;

		public override float Initial => 0.01f;

		public override float Increase => 0.005f;

		public override LocalizedText Description => base.Description.WithFormatArgs(Initial.ToPercent(), DamageIncrease.ToPercent());

		public override string UIInfo()
		{
			return UIInfoText.Format(Math.Min(RollChance, 1f).ToPercent(), 50 - killCount % 50);
		}

		//Original, todo
		//const int Initial = 6;
		//const int Increase = 2;
		//public override LocalizedText Description => base.Description.WithFormatArgs(Initial + Increase, damageIncrease.ToPercent()) $"Fire {0} fireworks that deal {1} damage";
		/*
		 * 
		 for loop up to Initial + Increase * Stack
			//Vector2 velo = new Vector2(Main.rand.NextFloat(-0.25f, 0.25f), -2f);
			//RandomMovementProj.NewProjectile<BundleOfFireworksProj>(GetEntitySource(player), target.Center, velo, damage, 10f);
		 * 
		 */

		//public override bool AlwaysProc => false;

		private float RollChance => Formula();

		public void OnKillNPCWithItem(Player player, Item item, NPC target, NPC.HitInfo hit, int damageDone)
		{
			killCount++;
			SpawnProjectile(target, player);
		}

		public void OnKillNPCWithProj(Player player, Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
		{
			killCount++;
			SpawnProjectile(target, player);
		}

		void SpawnProjectile(NPC target, Player player)
		{
			if (NPCHelper.IsSpawnedFromStatue(target))
			{
				killCount--;
				return; //No statue abuse for more fireworks
			}

			if (target.boss || killCount % 50 == 0 || Proc(RollChance))
			{
				int damage = (int)(DamageIncrease * player.GetDamage());
				SoundEngine.PlaySound(SoundID.Item13.WithVolumeScale(SoundHelper.FixVolume(2f)).WithPitchOffset(0.4f), player.Center);
				int count = fireworkCount;
				if (target.boss)
				{
					count *= 3;
				}
				for (int i = 0; i < count; i++)
				{
					Vector2 velo = new Vector2(Main.rand.NextFloat(-0.25f, 0.25f), -2f);
					RandomMovementProj.NewProjectile<BundleOfFireworksProj>(GetEntitySource(player), player.Center, velo, damage, 10f);
				}
			}

			if (killCount % 50 == 0) killCount = 0;
		}
	}
}
