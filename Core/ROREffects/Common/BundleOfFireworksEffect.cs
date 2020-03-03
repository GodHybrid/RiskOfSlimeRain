﻿using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.Misc;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Projectiles;
using Terraria;
using Terraria.ID;

namespace RiskOfSlimeRain.Core.ROREffects.Common
{
	public class BundleOfFireworksEffect : RORCommonEffect, IOnKill
	{
		const float initial = 0.005f;
		const float increase = 0.005f;
		const float damageIncrease = 9f;
		const int fireworkCount = 5;

		public override string Description => $" {(initial + increase).ToPercent()} chance to fire {fireworkCount} fireworks that deal {damageIncrease.ToPercent()} damage";

		public override string FlavorText => "Disguising homing missiles as fireworks? \nDon't ever quote me on it, but it was pretty smart";

		//Original, todo
		//const int initial = 6;
		//const int increase = 2;
		//public override string Description => $"Fire {initial + increase} fireworks that deal {damageIncrease.ToPercent()} damage";
		/*
		 * 
		 for loop up to initial + increase * Stack
			//Vector2 velo = new Vector2(Main.rand.NextFloat(-0.25f, 0.25f), -2f);
			//RandomMovementProj.NewProjectile<BundleOfFireworksProj>(target.Center, velo, damage, 10f);
		 * 
		 */

		public override bool AlwaysProc => false;

		public override float Chance => initial + increase * Stack;

		public void OnKillNPC(Player player, Item item, NPC target, int damage, float knockback, bool crit)
		{
			SpawnProjectile(target, player);
		}

		public void OnKillNPCWithProj(Player player, Projectile proj, NPC target, int damage, float knockback, bool crit)
		{
			SpawnProjectile(target, player);
		}

		void SpawnProjectile(NPC target, Player player)
		{
			if (MiscManager.IsSpawnedFromStatue(target)) return; //No statue abuse for more fireworks

			int damage = (int)(damageIncrease * player.GetDamage());
			SoundHelper.PlaySound(SoundID.Item13.SoundId, (int)player.Center.X, (int)player.Center.Y, SoundID.Item13.Style, SoundHelper.FixVolume(2f), 0.4f);
			for (int i = 0; i < fireworkCount; i++)
			{
				Vector2 velo = new Vector2(Main.rand.NextFloat(-0.25f, 0.25f), -2f);
				RandomMovementProj.NewProjectile<BundleOfFireworksProj>(player.Center, velo, damage, 10f);
			}
		}
	}
}
