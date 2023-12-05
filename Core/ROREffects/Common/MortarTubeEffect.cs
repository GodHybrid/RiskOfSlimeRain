using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Projectiles;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Core.ROREffects.Common
{
	public class MortarTubeEffect : RORCommonEffect, IOnHit
	{
		//const float Increase = 1.7f;

		public override float Initial => ServerConfig.Instance.OriginalStats ? 1.7f : 1.2f;

		public override float Increase => ServerConfig.Instance.OriginalStats ? 1.7f : 1.2f;

		public override string Description => $"{Chance.ToPercent()} chance to fire a mortar for {Initial.ToPercent()} damage";

		public override string FlavorText => "You stick explosives down the end, then you fire the explosive\nI suppose you can beat them with the tube afterwards";

		public override string UIInfo()
		{
			return $"Damage: {Formula().ToPercent()}";
		}

		public override bool AlwaysProc => false;

		public override float Chance => 0.09f;

		public void OnHitNPC(Player player, Item item, NPC target, NPC.HitInfo hit, int damageDone)
		{
			SpawnProjectile(player);
		}

		public void OnHitNPCWithProj(Player player, Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
		{
			SpawnProjectile(player);
		}

		void SpawnProjectile(Player player)
		{
			int damage = (int)(Formula() * player.GetDamage());
			Projectile.NewProjectile(GetEntitySource(player), player.Center - new Vector2(0, player.height >> 1), new Vector2(5 * player.direction, -5), ModContent.ProjectileType<MortarTubeRocket>(), 0, 0, Main.myPlayer, damage);
		}
	}
}
