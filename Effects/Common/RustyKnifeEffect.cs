using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Effects.Interfaces;
using RiskOfSlimeRain.Projectiles;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Effects.Common
{
	public class RustyKnifeEffect : ROREffect, IOnHit
	{
		public override int MaxStack => 7;

		public override string Description => "15% chance to cause bleeding";

		public override string FlavorText => "Murder weapon, case name ELIAS. Probably a lover's spat?\nThere is still dried blood on the knife, so mark it as biological.";

		public override float Chance => Stack * 0.15f;

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
			//first check if there is no projectile already sticking to the target
			for (int i = 0; i < Main.maxProjectiles; i++)
			{
				Projectile p = Main.projectile[i];
				if (p.active && p.type == ModContent.ProjectileType<RustyKnifeProj>() && p.ai[1] == target.whoAmI)
				{
					return;
				}
			}
			int damage = (int)(0.35f * player.GetWeaponDamage(player.HeldItem));
			Projectile.NewProjectile(target.Center, Vector2.Zero, ModContent.ProjectileType<RustyKnifeProj>(), 0, 0, Main.myPlayer, damage, target.whoAmI);
		}
	}
}
