using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Projectiles;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.World.Generation;

namespace RiskOfSlimeRain.Core.ROREffects.Uncommon
{
	public class FrostRelicEffect : RORUncommonEffect, IOnKill, IPostUpdateEquips
	{
		private int maxDuration = (int)(60 * 6.5f);
		private float radius = 64f;
		private bool active = false;

		public bool IsActive => active;

		private int Timer = 0;

		public override float Initial => ServerConfig.Instance.OriginalStats ? 3f : 3f;

		public override float Increase => 1f;

		public override string Description => $"Killing an enemy surrounds you with {Initial} icicle{(Initial > 1 ? "s" : "")} that deal{(Initial == 1 ? "s" : "")} 33% damage each.";

		public override string FlavorText => "Biggest snowflake, to date.\nTechnically, it isn't a snowflake, it's just a chunk of ice, but STILL.";

		public int IciclesSpawned => (int)Formula();

		public override string UIInfo()
		{
			return $"Icicles spawned per kill: {IciclesSpawned}";
		}

		public bool GetActiveState()
		{
			return IsActive;
		}

		public void OnKillNPC(Player player, Item item, NPC target, int damage, float knockback, bool crit)
		{
			SpawnProjectile(player, damage);
		}

		public void OnKillNPCWithProj(Player player, Projectile proj, NPC target, int damage, float knockback, bool crit)
		{
			SpawnProjectile(player, damage);
		}

		void SpawnProjectile(Player player, int damage)
		{
			//Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<FrostRelicProj>(), damage, 0, Main.myPlayer, player.GetDamage());
			if (!IsActive)
			{
				int division = 360 / IciclesSpawned;
				if(IciclesSpawned < 10)
					for (int i = 0; i < IciclesSpawned; i++)
					{
							CirclingMovementProj.NewProjectile<FrostRelicProj>(player.Center, damage, 0, radius: radius, angle: (int)(i * 360 / IciclesSpawned));
					}
				else if(IciclesSpawned < 25)
					for (int i = 0; i < IciclesSpawned; i++)
					{
						if (i % 2 == 0) CirclingMovementProj.NewProjectile<FrostRelicProj>(player.Center, damage, 0, radius: radius, angle: (int)(i * 360 / IciclesSpawned));
						else CirclingMovementProj.NewProjectile<FrostRelicProj>(player.Center, damage, 0, radius: 2 * radius, angle: (int)(i * 360 / IciclesSpawned));
					}
				active = true;
			}
			Timer = 0;
		}

		void IPostUpdateEquips.PostUpdateEquips(Player player)
		{
			if (IsActive) 
			{
				Timer++;
				if (Timer >= maxDuration)
				{
					Timer = 0;
					active = false;
				}
			}
		}
	}
}
