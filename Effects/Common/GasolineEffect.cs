using RiskOfSlimeRain.Effects.Interfaces;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Projectiles;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Effects.Common
{
	public class GasolineEffect : RORCommonEffect, IOnHit
	{
		const float initial = 0.2f;
		const float increase = 0.4f;

		public override string Description => $"Killing enemies burns the ground to deal {(initial + increase).ToPercent()} damage and set enemies on fire";

		public override string FlavorText => "Gasoline, eh?\nSurprising to find a gas station these days, with everyone drivin' around them electro cars";

		public void OnHitNPC(Player player, Item item, NPC target, int damage, float knockback, bool crit)
		{
			SpawnProjectile(player, target);
		}

		public void OnHitNPCWithProj(Player player, Projectile proj, NPC target, int damage, float knockback, bool crit)
		{
			SpawnProjectile(player, target);
		}

		void SpawnProjectile(Player player, NPC target)
		{
			if (target.life <= 0)
			{
				int extraPerSide = 3;

				for (int x = -extraPerSide; x < 1 + extraPerSide; x++)
				{
					int spawnLight = x == 0 ? 1 : 0;

					Projectile.NewProjectile(target.Center.X, target.Center.Y, x, 1, ModContent.ProjectileType<FireProj>(), (int)((initial + (Stack * increase)) * player.GetDamage()), 0, Main.myPlayer, 0, spawnLight);
				}
			}
		}
	}
}
