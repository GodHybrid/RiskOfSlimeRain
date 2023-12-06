using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Projectiles;
using Terraria;
using Terraria.Localization;

namespace RiskOfSlimeRain.Core.ROREffects.Common
{
	public class StickyBombEffect : RORCommonEffect, IOnHit
	{
		//const float Initial = 1.0f;
		//const float Increase = 0.4f;
		const int bound = 4;

		public override float Initial => ServerConfig.Instance.OriginalStats ? 1.4f : 1.2f;

		public override float Increase => ServerConfig.Instance.OriginalStats ? 0.4f : 0.3f;

		public override LocalizedText Description => base.Description.WithFormatArgs(Chance.ToPercent(), Initial.ToPercent());

		public override string UIInfo()
		{
			return $"Damage: {Formula().ToPercent()}";
		}

		public override bool AlwaysProc => false;

		public override float Chance => 0.08f;

		public void OnHitNPC(Player player, Item item, NPC target, NPC.HitInfo hit, int damageDone)
		{
			SpawnProjectile(player, target);
		}

		public void OnHitNPCWithProj(Player player, Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
		{
			SpawnProjectile(player, target);
		}

		void SpawnProjectile(Player player, NPC target)
		{
			int damage = (int)(Formula() * player.GetDamage());
			int width = Main.rand.Next(target.width);
			int height = bound;
			if ((target.height >> 1) > bound)
			{
				height = Main.rand.Next(bound, target.height - bound);
			}
			Vector2 offset = new Vector2(width, height);
			StickyProj.NewProjectile<StickyBombProj>(GetEntitySource(player), target, offset, damage);
		}
	}
}
