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
		//const float Initial = 0.05f;
		//const float Increase = 0.05f;
		
		public override float Initial => 0.1f;

		public override float Increase => 0.05f;

		public override int MaxRecommendedStack => 7;

		public override bool EnforceMaxStack => true;

		public override string Name => "Hermit's Scarf";

		public override string Description => $"Allows you to evade attacks with {Initial.ToPercent()} chance";

		public override string FlavorText => "This thing survived that horrible day\nIt must be able to survive whatever I have to endure, right?";

		public override string UIInfo()
		{
			return $"Chance: {Chance}";
		}

		public override bool AlwaysProc => false;

		public override float Chance => Formula();

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
