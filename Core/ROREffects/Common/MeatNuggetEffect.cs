using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;

namespace RiskOfSlimeRain.Core.ROREffects.Common
{
	public class MeatNuggetEffect : RORCommonEffect, IOnHit
	{
		//const int Increase = 3;

		public override float Initial => 6f;

		public override float Increase => 6f;

		public override LocalizedText Description => base.Description.WithFormatArgs(Chance.ToPercent(), Initial);

		public override string UIInfo()
		{
			return UIInfoText.Format(Formula());
		}

		public override bool AlwaysProc => false;

		public override float Chance => 0.08f;

		public void OnHitNPCWithItem(Player player, Item item, NPC target, NPC.HitInfo hit, int damageDone)
		{
			SpawnProjectile(player, target);
		}

		public void OnHitNPCWithProj(Player player, Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
		{
			SpawnProjectile(player, target);
		}

		void SpawnProjectile(Player player, NPC target)
		{
			//Prevent abuse on dummies
			if (target.type == NPCID.TargetDummy) return;

			Projectile.NewProjectile(GetEntitySource(player), target.Center, new Vector2(Main.rand.Next(-5, 5), Main.rand.Next(-5, -2)), ModContent.ProjectileType<MeatNuggetProj>(), 0, 0, Main.myPlayer, Formula());
		}
	}
}
