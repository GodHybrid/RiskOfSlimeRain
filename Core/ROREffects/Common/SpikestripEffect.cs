using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Projectiles;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Core.ROREffects.Common
{
	public class SpikestripEffect : RORCommonEffect, IPostHurt
	{
		const int initial = 60;
		const int increase = 60;
		//effect takes place in the RORGlobalNPC with different values
		const float slow = 0.2f;

		public override string Description => $"Drop spikestrips on hit, slowing enemies by {slow.ToPercent()}";

		public override string FlavorText => "The doctors say I don't have much time left\nSince you're in the force now and all, I felt obligated to return it to you";

		public void PostHurt(Player player, bool pvp, bool quiet, double damage, int hitDirection, bool crit)
		{
			Projectile.NewProjectile(player.position, new Vector2(0, 0), ModContent.ProjectileType<SpikestripProj>(), 0, 0, Main.myPlayer, initial + Stack * increase);
			Projectile.NewProjectile(player.position, new Vector2(2, 0), ModContent.ProjectileType<SpikestripProj>(), 0, 0, Main.myPlayer, initial + Stack * increase);
			Projectile.NewProjectile(player.position, new Vector2(-2, 0), ModContent.ProjectileType<SpikestripProj>(), 0, 0, Main.myPlayer, initial + Stack * increase);
		}
	}
}
