using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Effects.Interfaces;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Projectiles;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Effects.Common
{
	public class MortarTubeEffect : ROREffect, IOnHit
	{
		const float increase = 1.7f;

		public override string Description => $"{Chance.ToPercent()} chance to fire a mortar for {increase.ToPercent()} damage";

		public override string FlavorText => "You stick explosives down the end, then you fire the explosive\nI suppose you can beat them with the tube afterwards";

		public override bool AlwaysProc => false;

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
			Projectile.NewProjectile(player.Center - new Vector2(0, player.height >> 1), new Vector2(5 * player.direction, -5), ModContent.ProjectileType<MortarTubeRocket>(), 0, 0, Main.myPlayer, (int)(player.GetDamage() * increase * Stack));
		}
	}
}
