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
		//const int Initial = 60;
		//const int Increase = 60;
		//Effect takes place in the RORGlobalNPC with different values
		const float slow = 0.2f;
		const int count = 3;

		public override float Initial => 2f;

		public override float Increase => 1f;

		public override string Description => $"Drop spikestrips on hit, slowing enemies by {slow.ToPercent()}";

		public override string FlavorText => "The doctors say I don't have much time left\nSince you're in the force now and all, I felt obligated to return it to you";

		public override string UIInfo()
		{
			return $"Spikestrip lifetime: {Formula()}s";
		}

		public void PostHurt(Player player, bool pvp, bool quiet, double damage, int hitDirection, bool crit)
		{
			for (int i = -1; i < count - 1; i++)
			{
				Projectile.NewProjectile(player.Center, new Vector2(2 * i, 0), ModContent.ProjectileType<SpikestripProj>(), 0, 0, Main.myPlayer, (int)Formula() * 60);
			}
		}
	}
}
