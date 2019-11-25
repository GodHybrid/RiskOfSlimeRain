﻿using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Buffs;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Projectiles
{
	public class SpikestripStrip : ModProjectile
	{
		public override void SetDefaults()
		{
			projectile.width = 34;
			projectile.height = 4;
			projectile.friendly = true;
			projectile.penetrate = -1;
			//projectile.tileCollide = true;
			projectile.timeLeft = 300;
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			projectile.velocity = new Vector2(0f, 0f);
			return false;
		}

		public override void AI()
		{
			projectile.velocity.Y = 10f;
			foreach (NPC enemy in Main.npc)
			{
				if (enemy.Hitbox.Intersects(projectile.Hitbox))
				{
					enemy.AddBuff(ModContent.BuffType<SpikestripSlowdown>(), 60);
				}
			}
		}

		public override void Kill(int timeLeft)
		{

		}
	}
}
