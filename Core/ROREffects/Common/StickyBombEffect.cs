using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Projectiles;
using Terraria;

namespace RiskOfSlimeRain.Core.ROREffects.Common
{
	public class StickyBombEffect : RORCommonEffect, IOnHit
	{
		//const float Initial = 1.0f;
		//const float Increase = 0.4f;
		const int bound = 4;

		public override float Initial => ServerConfig.Instance.RorStats ? 1.4f : 1.2f;

		public override float Increase => ServerConfig.Instance.RorStats ? 0.4f : 0.3f;

		public override string Description => $"{Chance.ToPercent()} chance to attach a bomb to an enemy, detonating for {Initial.ToPercent()} damage";

		public override string FlavorText => "Once you take the wrapping off, the adhesive is ACTIVE. DON'T TOUCH IT.\nYOU STICK THAT END ON BAD THINGS, NOT YOURSELF";

		public override string UIInfo()
		{
			return $"Damage: {Formula().ToPercent()}";
		}

		public override bool AlwaysProc => false;

		public override float Chance => 0.08f;

		void IOnHit.OnHitNPC(Player player, Item item, NPC target, int damage, float knockback, bool crit)
		{
			SpawnProjectile(player, target);
		}

		void IOnHit.OnHitNPCWithProj(Player player, Projectile proj, NPC target, int damage, float knockback, bool crit)
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
			StickyProj.NewProjectile<StickyBombProj>(target, offset, damage);
		}
	}
}
