using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Buffs;
using RiskOfSlimeRain.Effects.Interfaces;
using RiskOfSlimeRain.Projectiles;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Effects.Common
{
	public class StickyBombEffect : ROREffect, IOnHit
	{
		const int initial = 60;
		const int increase = 30;

		public override string Description => "8% chance to attach a bomb to an enemy, detonating for 140% damage";

		public override string FlavorText => "Once you take the wrapping off, the adhesive is ACTIVE. DON'T TOUCH IT.\nYOU STICK THAT END ON BAD THINGS, NOT YOURSELF.";

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
			Projectile.NewProjectile(target.Center, new Vector2(Main.rand.Next(-5, 5), Main.rand.Next(-5, 1)), ModContent.ProjectileType<StickyBombProj>(), 0, 0, Main.myPlayer, (int)((initial + increase * Stack) * player.GetWeaponDamage(player.HeldItem)), target.whoAmI);
		}
	}
}
