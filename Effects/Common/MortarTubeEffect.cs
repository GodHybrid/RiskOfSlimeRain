using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Effects.Interfaces;
using RiskOfSlimeRain.Projectiles;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Effects.Common
{
	public class MortarTubeEffect : ROREffect, IOnHit
	{
		const float initial = 0.2f;
		const float increase = 0.4f;

		public override string Description => "9% chance to fire a mortar for 170% damage";

		public override string FlavorText => "You stick explosives down the end, then you fire the explosive.\nI suppose you can beat them with the tube afterwards.";

		public override float Chance => 0.09f;

		public void OnHitNPC(Player player, Item item, NPC target, int damage, float knockback, bool crit)
		{
			SpawnProjectile(player);
		}

		public void OnHitNPCWithProj(Player player, Projectile proj, NPC target, int damage, float knockback, bool crit)
		{
			SpawnProjectile(player);
		}

		void SpawnProjectile(Player player)
		{
			Projectile.NewProjectile(player.Center, new Vector2(5 * player.direction, -5), ModContent.ProjectileType<MortarRocket>(), (int)(player.GetWeaponDamage(player.HeldItem) * 1.7f * Stack), 0);
		}
	}
}
