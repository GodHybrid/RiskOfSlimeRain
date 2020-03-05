using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Projectiles;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Core.ROREffects.Common
{
	public class GasolineEffect : RORCommonEffect, IOnKill
	{
		const float initial = 0.2f;
		const float increase = 0.4f;

		const int npcKilledLimitBeforeSpawning = 5;

		public override string Description => $"Killing enemies burns the ground to deal {(initial + increase).ToPercent()} damage and set enemies on fire";

		public override string FlavorText => "Gasoline, eh?\nSurprising to find a gas station these days, with everyone drivin' around them electro cars";

		public void OnKillNPC(Player player, Item item, NPC target, int damage, float knockback, bool crit)
		{
			SpawnProjectile(player, target);
		}

		public void OnKillNPCWithProj(Player player, Projectile proj, NPC target, int damage, float knockback, bool crit)
		{
			SpawnProjectile(player, target);
		}

		void SpawnProjectile(Player player, NPC target)
		{
			int extraPerSide = 3;

			int type = ModContent.ProjectileType<FireProj>();

			if (player.ownedProjectileCounts[type] >= npcKilledLimitBeforeSpawning * (1 + 2 * extraPerSide)) return;

			int damage = (int)((initial + (Stack * increase)) * player.GetDamage());

			int count = 0;
			for (int x = -extraPerSide; x < 1 + extraPerSide; x++)
			{
				int spawnLight = x == 0 ? 1 : 0;

				Projectile.NewProjectile(target.Center.X, target.Center.Y, x, 1, type, damage, 0, Main.myPlayer, 0, spawnLight);
				count++;
			}
		}
	}
}
