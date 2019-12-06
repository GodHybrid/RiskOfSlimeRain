using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Effects.Interfaces;
using RiskOfSlimeRain.Projectiles;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Effects.Common
{
	public class MeatNuggetEffect : ROREffect, IOnHit
	{
		const int increase = 6;

		public override string Description => "Upon use enemies will have an 8% chance to drop two meat nuggets\nEach meat nugget recovers 6 health";

		public override string FlavorText => "MM. Delicious\nJust kidding, it's awful";

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
			for (int i = 0; i < 2; i++)
				Projectile.NewProjectile(target.Center, new Vector2(Main.rand.Next(-5, 5), Main.rand.Next(-5, 1)), ModContent.ProjectileType<MeatNugget>(), 0, 0, Main.myPlayer, Stack * increase);
		}
	}
}
