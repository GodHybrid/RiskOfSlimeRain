using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Effects.Interfaces;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Projectiles;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Effects.Common
{
	public class LensmakersGlassesEffect : ROREffect, IModifyHit, IOnHit
	{
		const float increase = 0.07f;

		public override int MaxRecommendedStack => 14;

		public override string Name => "Lens-Maker's Glasses";

		public override string Description => $"Increases crit chance by {increase.ToPercent()}";

		public override string FlavorText => "Calibrated for high focal alignment\nShould allow for the precision you were asking for";

		public override bool AlwaysProc => false;

		public override float Chance => increase * Stack;

		public void ModifyHitNPC(Player player, Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
		{
			crit = true;
		}

		public void ModifyHitNPCWithProj(Player player, Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			crit = true;
		}

		public void OnHitNPC(Player player, Item item, NPC target, int damage, float knockback, bool crit)
		{
			if (crit)
			{
				Projectile.NewProjectile(target.Center, Vector2.Zero, ModContent.ProjectileType<LensmakersGlassesProj>(), 0, 0, Main.myPlayer, 0, target.whoAmI);
			}
		}

		public void OnHitNPCWithProj(Player player, Projectile proj, NPC target, int damage, float knockback, bool crit)
		{
			if (crit)
			{
				Projectile.NewProjectile(target.Center, Vector2.Zero, ModContent.ProjectileType<LensmakersGlassesProj>(), 0, 0, Main.myPlayer, 0, target.whoAmI);
			}
		}
	}
}
