using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Effects.Interfaces;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Effects.Common
{
	public class MeatNuggetEffect : RORCommonEffect, IOnHit
	{
		const int increase = 6;

		public override string Description => $"Enemies will have an {Chance.ToPercent()} chance to drop two meat nuggets\nEach meat nugget recovers {increase} health";

		public override string FlavorText => "MM. Delicious\nJust kidding, it's awful";

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
			if (target.type == NPCID.TargetDummy) return;
			for (int i = 0; i < 2; i++)
				Projectile.NewProjectile(target.Center, new Vector2(Main.rand.Next(-5, 5), Main.rand.Next(-5, 1)), ModContent.ProjectileType<MeatNugget>(), 0, 0, Main.myPlayer, Stack * increase);
		}
	}
}
