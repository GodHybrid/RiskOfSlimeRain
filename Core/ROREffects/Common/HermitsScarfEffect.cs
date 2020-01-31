using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Projectiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Core.ROREffects.Common
{
	public class HermitsScarfEffect : RORCommonEffect, IPreHurt
	{
		const float initial = 0.05f;
		const float increase = 0.05f;

		public override int MaxRecommendedStack => 7;

		public override string Name => "Hermit's Scarf";

		public override string Description => $"Allows you to evade attacks with {(initial + increase).ToPercent()} chance";

		public override string FlavorText => "This thing survived that horrible day\nIt must be able to survive whatever I have to endure, right?";

		public override bool AlwaysProc => false;

		public override float Chance => initial + increase * Stack;

		public bool PreHurt(Player player, bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
		{
			damage = 0;
			player.immune = true;
			player.immuneTime = 60;
			Projectile.NewProjectile(player.Center, new Vector2(0, -0.3f), ModContent.ProjectileType<HermitsScarfProj>(), 0, 0, Main.myPlayer);
			return false;
		}
	}
}
