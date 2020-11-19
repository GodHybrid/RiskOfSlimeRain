using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Projectiles;
using System;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Core.ROREffects.Uncommon
{
	public class DeadMansFootEffect : RORUncommonEffect, IPostHurt
	{
		public override float Initial => 4;

		public override float Increase => 1;

		public const float damage = 1.5f;
		public const int damageLow = 30;
		public const float damageHigh = 0.175f;

		private float Ticks => Formula();

		public override string Description => $"Drop a poison mine at low health or after taking significant damage for {Initial}x{damage.ToPercent()} damage";

		public override string FlavorText => "It looks like he was infested by some bug-like creatures, and exploded when I got close.\nI hope his death wasn't too painful; his family will know how he died.";

		public override string Name => "Dead Man's Foot";

		public override string UIInfo()
		{
			return $"Debuff duration: {30 * (int)Formula()}";
		}

		public void PostHurt(Player player, bool pvp, bool quiet, double damage, int hitDirection, bool crit)
		{
			if (Main.myPlayer == player.whoAmI && damage >= Math.Max(damageLow, (int)(player.statLifeMax2 * damageHigh)) || player.statLife <= (int)(player.statLifeMax2 * 0.15f))
			{
				Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<DeadMansFootMineProj>(), 0, 0, Main.myPlayer, player.GetDamage(), Ticks);
			}
		}
	}
}
