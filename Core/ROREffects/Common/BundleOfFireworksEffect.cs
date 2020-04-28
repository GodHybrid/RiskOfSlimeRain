using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Projectiles;
using System;
using Terraria;
using Terraria.ID;

namespace RiskOfSlimeRain.Core.ROREffects.Common
{
	public class BundleOfFireworksEffect : RORCommonEffect, IOnKill
	{
		//const float Initial = 0.005f;
		//const float Increase = 0.005f;
		const int fireworkCount = 5;

		private float DamageIncrease => ServerConfig.Instance.RorStats ? 9f : 7f;

		public override float Initial => 0.01f;

		public override float Increase => 0.005f;

		public override string Description => $" {Initial.ToPercent()} chance to fire {fireworkCount} fireworks that deal {DamageIncrease.ToPercent()} damage";

		public override string FlavorText => "Disguising homing missiles as fireworks?\nDon't ever quote me on it, but it was pretty smart";

		public override string UIInfo()
		{
			return $"Chance: {Math.Min(Chance, 1f).ToPercent()}";
		}

		//Original, todo
		//const int Initial = 6;
		//const int Increase = 2;
		//public override string Description => $"Fire {Initial + Increase} fireworks that deal {damageIncrease.ToPercent()} damage";
		/*
		 * 
		 for loop up to Initial + Increase * Stack
			//Vector2 velo = new Vector2(Main.rand.NextFloat(-0.25f, 0.25f), -2f);
			//RandomMovementProj.NewProjectile<BundleOfFireworksProj>(target.Center, velo, damage, 10f);
		 * 
		 */

		public override bool AlwaysProc => false;

		public override float Chance => Formula();

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
			if (NPCHelper.IsSpawnedFromStatue(target)) return; //No statue abuse for more fireworks

			int damage = (int)(DamageIncrease * player.GetDamage());
			SoundHelper.PlaySound(SoundID.Item13.SoundId, (int)player.Center.X, (int)player.Center.Y, SoundID.Item13.Style, SoundHelper.FixVolume(2f), 0.4f);
			for (int i = 0; i < fireworkCount; i++)
			{
				Vector2 velo = new Vector2(Main.rand.NextFloat(-0.25f, 0.25f), -2f);
				RandomMovementProj.NewProjectile<BundleOfFireworksProj>(player.Center, velo, damage, 10f);
			}
		}
	}
}
