using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Projectiles;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WebmilioCommons.Tinq;

namespace RiskOfSlimeRain.Core.ROREffects.Boss
{
	public class BurningWitnessEffect : RORBossEffect, IPostUpdateEquips, IModifyHit, IOnHit, IPostUpdateRunSpeeds
	{
		const int initialDuration = 6;
		const int durationIncrease = 3;

		int timer = -1;

		//Initial/Increase are for the movement speed boost
		public override float Initial => ServerConfig.Instance.OriginalStats ? 0.05f : 0.04f;

		public override float Increase => ServerConfig.Instance.OriginalStats ? 0.05f : 0.02f;

		int Duration => initialDuration + durationIncrease * Math.Max(0, Stack - 1);

		public override string Description => $"Grant +{Initial.ToPercent()} movement speed, +1 damage, and a firetrail on kill for {initialDuration} seconds";

		public override string FlavorText => "The Worm's eye seems to still see... watching... rewarding...";

		public override string UIInfo()
		{
			return $"Current speed increase: {Formula().ToPercent()}. Current duration: {Duration}";
		}

		//To restrict rapidly spawning projectiles optop of eachother, used in inflating the players hitbox alot after spawning one
		int spawnTimer = 0;

		public void PostUpdateEquips(Player player)
		{
			if (timer >= 0)
			{
				timer--;

				if (Main.netMode == NetmodeID.Server || player.whoAmI != Main.myPlayer) return;
				if (Main.netMode == NetmodeID.MultiplayerClient && timer % 2 == 1) return; //Half the projectile spam
				if (player.velocity.Y != 0 || player.oldVelocity.Y != 0 || player.grappling[0] > -1) return;

				if (spawnTimer > 0)
				{
					spawnTimer--;
				}

				Rectangle hitbox = player.Hitbox;
				hitbox.Inflate(spawnTimer, spawnTimer);
				bool noTrail = !Main.projectile.AnyActive(p => p.Hitbox.Intersects(hitbox));
				int fireDuration = 50;
				if (noTrail || timer % (fireDuration - 10) == 0)
				{
					int type = ModContent.ProjectileType<FireProj>();
					int damage = (int)(0.35f * player.GetDamage());

					Projectile.NewProjectile(player.Center.X - player.direction * player.width / 2, player.Center.Y + 6f, 0, 1, type, damage, 0, Main.myPlayer, fireDuration);
					spawnTimer = 20;
				}
			}
		}

		public void PostUpdateRunSpeeds(Player player)
		{
			player.maxRunSpeed += player.maxRunSpeed * Formula();
			player.moveSpeed += player.moveSpeed * Formula();
		}

		public void ModifyHitNPC(Player player, Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
		{
			if (timer > 0) damage += 1;
		}

		public void ModifyHitNPCWithProj(Player player, Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			if (timer > 0) damage += 1;
		}

		public void OnHitNPC(Player player, Item item, NPC target, int damage, float knockback, bool crit)
		{
			if (target.life <= 0) timer = Duration * 60;
		}

		public void OnHitNPCWithProj(Player player, Projectile proj, NPC target, int damage, float knockback, bool crit)
		{
			if (target.life <= 0) timer = Duration * 60;
		}
	}
}
