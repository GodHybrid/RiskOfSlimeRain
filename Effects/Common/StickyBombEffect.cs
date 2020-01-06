using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Effects.Interfaces;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Projectiles;
using Terraria;

namespace RiskOfSlimeRain.Effects.Common
{
	public class StickyBombEffect : ROREffect, IOnHit
	{
		const float initial = 1.0f;
		const float increase = 0.4f;

		public override string Description => $"{Chance.ToPercent()} chance to attach a bomb to an enemy, detonating for {(initial + increase).ToPercent()} damage";

		public override string FlavorText => "Once you take the wrapping off, the adhesive is ACTIVE. DON'T TOUCH IT.\nYOU STICK THAT END ON BAD THINGS, NOT YOURSELF";

		public override bool AlwaysProc => false;

		public override float Chance => 0.08f;

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
			int damage = (int)((initial + increase * Stack) * player.GetDamage());
			Vector2 offset = new Vector2(Main.rand.Next(target.width), Main.rand.Next(4, target.height - 4));
			StickyProj.NewProjectile<StickyBombProj>(target, offset, damage);
			//uint packedOffset = GetPackedOffset(new Point(Main.rand.Next(target.width), Main.rand.Next(4, target.height - 4)));
			//int index = Projectile.NewProjectile(target.Center, Vector2.Zero, ModContent.ProjectileType<StickyBombProj>(), 0, 0, Main.myPlayer, packedOffset, target.whoAmI);
			//if (index > -1)
			//{
			//	Projectile proj = Main.projectile[index];
			//	int damage = (int)((initial + increase * Stack) * player.GetDamage());
			//	proj.localAI[0] = damage;
			//	//this doesnt need syncing cause the damage is for spawning another projectile, which is clientside
			//}
		}
	}
}
