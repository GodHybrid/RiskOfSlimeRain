using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Projectiles;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Core.ROREffects.Common
{
	//It doesn't use IGetWeaponCrit because we want crit rate to be dynamic based on item use time (which requires proc chance)
	//additional crit chance won't be shown on the tooltip
	public class LensmakersGlassesEffect : RORCommonEffect, IModifyHit, IOnHit, IPlayerLayer
	{
		const float increase = 0.07f;

		public override int MaxRecommendedStack => 14;

		public override string Name => "Lens-Maker's Glasses";

		public override string Description => $"Increases crit chance by {increase.ToPercent()}";

		public override string FlavorText => "Calibrated for high focal alignment\nShould allow for the precision you were asking for";

		public override bool AlwaysProc => true;

		public PlayerLayerParams GetPlayerLayerParams(Player player)
		{
			return new PlayerLayerParams("Textures/LensMakersGlasses", new Vector2(0, -50));
		}

		public void ModifyHitNPC(Player player, Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
		{
			if (!Proc(increase * Stack)) return;
			crit = true;
		}

		public void ModifyHitNPCWithProj(Player player, Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			if (!Proc(increase * Stack)) return;
			crit = true;
		}

		public void OnHitNPC(Player player, Item item, NPC target, int damage, float knockback, bool crit)
		{
			SpawnProjectile(target, crit);
		}

		public void OnHitNPCWithProj(Player player, Projectile proj, NPC target, int damage, float knockback, bool crit)
		{
			SpawnProjectile(target, crit);
		}

		void SpawnProjectile(NPC target, bool crit)
		{
			if (crit)
			{
				Projectile.NewProjectile(target.Center, Vector2.Zero, ModContent.ProjectileType<LensmakersGlassesProj>(), 0, 0, Main.myPlayer);
			}
		}
	}
}
