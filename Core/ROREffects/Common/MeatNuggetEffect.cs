﻿using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Core.ROREffects.Common
{
	public class MeatNuggetEffect : RORCommonEffect, IOnHit
	{
		//const int Increase = 3;
		const int nuggetCount = 2;

		public override float Initial => 3f;

		public override float Increase => 3f;

		public override string Description => $"Enemies will have a {Chance.ToPercent()} chance to drop two meat nuggets\nEach meat nugget recovers {Initial} health";

		public override string FlavorText => "MM. Delicious\nJust kidding, it's awful";
		
		public override string UIInfo()
		{
			return $"Heal amount per nugget: {Formula() / nuggetCount}";
		}

		public override bool AlwaysProc => false;

		public override float Chance => 0.08f;

		public void OnHitNPC(Player player, Item item, NPC target, int damage, float knockback, bool crit)
		{
			SpawnProjectile(target);
		}

		public void OnHitNPCWithProj(Player player, Projectile proj, NPC target, int damage, float knockback, bool crit)
		{
			SpawnProjectile(target);
		}

		void SpawnProjectile(NPC target)
		{
			//prevent abuse on dummies
			if (target.type == NPCID.TargetDummy) return;

			for (int i = 0; i < nuggetCount; i++)
			{
				Projectile.NewProjectile(target.Center, new Vector2(Main.rand.Next(-5, 5), Main.rand.Next(-5, 0)), ModContent.ProjectileType<MeatNuggetProj>(), 0, 0, Main.myPlayer, Formula());
			}
		}
	}
}
