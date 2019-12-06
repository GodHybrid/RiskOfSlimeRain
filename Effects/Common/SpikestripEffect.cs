using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Effects.Interfaces;
using RiskOfSlimeRain.Projectiles;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Effects.Common
{
	public class SpikestripEffect : ROREffect, IPostHurt
	{
		const int initial = 60;
		const int increase = 60;

		public override string Description => "Drop spikestrips on hit, slowing enemies by 20%";

		public override string FlavorText => "The doctors say I don't have much time left\nSince you're in the force now and all, I felt obligated to return it to you";

		public void PostHurt(Player player, bool pvp, bool quiet, double damage, int hitDirection, bool crit)
		{
			Projectile.NewProjectile(player.position, new Vector2(0, 0), ModContent.ProjectileType<SpikestripStrip>(), initial + Stack * increase, 0, Main.myPlayer);
			Projectile.NewProjectile(player.position, new Vector2(2, 0), ModContent.ProjectileType<SpikestripStrip>(), initial + Stack * increase, 0, Main.myPlayer);
			Projectile.NewProjectile(player.position, new Vector2(-2, 0), ModContent.ProjectileType<SpikestripStrip>(), initial + Stack * increase, 0, Main.myPlayer);
		}
	}
}
