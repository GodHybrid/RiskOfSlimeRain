using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Core.ROREffects.Common
{
	public class MeatNuggetEffect : RORCommonEffect, IOnHit
	{
		//const int Increase = 3;

		public override float Initial => 6f;

		public override float Increase => 6f;

		public override string Description => $"Enemies will have a {Chance.ToPercent()} chance to drop a meat nugget that recovers {Initial} health";

		public override string FlavorText => "MM. Delicious\nJust kidding, it's awful";

		public override string UIInfo()
		{
			return $"Heal amount: {Formula()}";
		}

		public override bool AlwaysProc => false;

		public override float Chance => 0.08f;

		public void OnHitNPC(Player player, Item item, NPC target, int damage, float knockback, bool crit)
		{
			SpawnProjectile(target);
		}

		public void OnHitNPCWithProj(Player player, Projectile proj, NPC target, int damage, float knockback, bool crit)
		{
			SpawnProjectile(target);
		}

		void SpawnProjectile(NPC target)
		{
			//Prevent abuse on dummies
			if (target.type == NPCID.TargetDummy) return;

			Projectile.NewProjectile(target.Center, new Vector2(Main.rand.Next(-5, 5), Main.rand.Next(-5, -2)), ModContent.ProjectileType<MeatNuggetProj>(), 0, 0, Main.myPlayer, Formula());
		}
	}
}
