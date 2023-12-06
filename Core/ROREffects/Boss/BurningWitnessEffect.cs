using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Projectiles;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Core.ROREffects.Boss
{
	public class BurningWitnessEffect : RORBossEffect, IPostUpdateEquips, IModifyHit, IOnHit, IPostUpdateRunSpeeds, IPlayerLayer
	{
		const int durationIncrease = 3;
		const int flatDamage = 1;
		int InitialDuration => ServerConfig.Instance.OriginalStats ? 6 : 3;

		int timer = -1;

		//Initial/Increase are for the movement speed boost
		public override float Initial => 0.05f;

		public override float Increase => ServerConfig.Instance.OriginalStats ? 0.05f : 0.02f;

		int Duration => InitialDuration + durationIncrease * Math.Max(0, Stack - 1);

		public override LocalizedText Description => base.Description.WithFormatArgs(Initial.ToPercent(), flatDamage, InitialDuration);

		public override string UIInfo()
		{
			return $"Current speed increase: {Formula().ToPercent()}. Current duration: {Duration}";
		}

		public PlayerLayerParams GetPlayerLayerParams(Player player)
		{
			return new PlayerLayerParams("Textures/BurningWitness", new Vector2(-12, -36));
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

				bool noExistingTrail = true;
				int type = ModContent.ProjectileType<FireProj>();
				for (int i = 0; i < Main.maxProjectiles; i++)
				{
					Projectile proj = Main.projectile[i];

					if (proj.active && proj.type == type && proj.Hitbox.Intersects(hitbox))
					{
						noExistingTrail = false;
						break;
					}
				}

				int fireDuration = 50;
				if (timer % (fireDuration - 10) == 0 || noExistingTrail)
				{
					int damage = (int)(0.35f * player.GetDamage());

					Projectile.NewProjectile(GetEntitySource(player), player.Center.X - player.direction * player.width / 2, player.Center.Y + 6f, 0, 1, type, damage, 0, Main.myPlayer, fireDuration);
					spawnTimer = 20;
				}
			}
		}

		public void PostUpdateRunSpeeds(Player player)
		{
			player.maxRunSpeed += player.maxRunSpeed * Formula();
			player.moveSpeed += player.moveSpeed * Formula();
		}

		public void ModifyHitNPC(Player player, Item item, NPC target, ref NPC.HitModifiers modifiers)
		{
			if (timer > 0) modifiers.FlatBonusDamage += flatDamage;
		}

		public void ModifyHitNPCWithProj(Player player, Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
		{
			if (timer > 0) modifiers.FlatBonusDamage += flatDamage;
		}

		public void OnHitNPCWithItem(Player player, Item item, NPC target, NPC.HitInfo hit, int damageDone)
		{
			if (target.life <= 0) timer = Duration * 60;
		}

		public void OnHitNPCWithProj(Player player, Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
		{
			if (target.life <= 0) timer = Duration * 60;
		}
	}
}
