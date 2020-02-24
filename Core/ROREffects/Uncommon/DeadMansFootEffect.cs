using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Projectiles;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Core.ROREffects.Uncommon
{
	//TODO Finish this lol kek
	public class DeadMansFootEffect : RORUncommonEffect, IPostHurt
	{
		public const float initial = 3;
		public const float increase = 1;
		public const float damage = 1.5f;

		public float Ticks => initial + increase * Stack;

		//public override bool AlwaysProc => true;
		public override string Description => $"Drop a poison mine at low health for {initial + increase}x{damage.ToPercent()} damage.";
		public override string FlavorText => "It looks like he was infested by some bug-like creatures, and exploded when I got close.\nI hope his death wasn't too painful; his family will know how he died.";
		public override string Name => "Dead Man's Foot";

		public void PostHurt(Player player, bool pvp, bool quiet, double damage, int hitDirection, bool crit)
		{
			if (damage >= 50 || player.statLife <= (int)(player.statLifeMax2 * 0.15f))
			{
				Projectile.NewProjectile(player.position, new Vector2(0, 0), ModContent.ProjectileType<DeadMansFootProj>(), 0, 0, Main.myPlayer, player.GetDamage(), Ticks);
			}
		}
	}
}
