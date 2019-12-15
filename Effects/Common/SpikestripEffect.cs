using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RiskOfSlimeRain.Effects.Interfaces;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.NPCs;
using RiskOfSlimeRain.Projectiles;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Effects.Common
{
	public class SpikestripEffect : ROREffect, IPostHurt/*, IPostDrawNPC*/
	{
		const int initial = 60;
		const int increase = 60;
		//effect takes place in the RORGlobalNPC with different values
		const float slow = 0.2f;

		public override string Description => $"Drop spikestrips on hit, slowing enemies by {slow.ToPercent()}";

		public override string FlavorText => "The doctors say I don't have much time left\nSince you're in the force now and all, I felt obligated to return it to you";

		public void PostHurt(Player player, bool pvp, bool quiet, double damage, int hitDirection, bool crit)
		{
			Projectile.NewProjectile(player.position, new Vector2(0, 0), ModContent.ProjectileType<SpikestripStrip>(), 0, 0, Main.myPlayer, initial + Stack * increase);
			Projectile.NewProjectile(player.position, new Vector2(2, 0), ModContent.ProjectileType<SpikestripStrip>(), 0, 0, Main.myPlayer, initial + Stack * increase);
			Projectile.NewProjectile(player.position, new Vector2(-2, 0), ModContent.ProjectileType<SpikestripStrip>(), 0, 0, Main.myPlayer, initial + Stack * increase);
		}

		//TODO draw manually for now
		public static void PostDraw(NPC npc, SpriteBatch spriteBatch, Color drawColor)
		{
			if (npc.GetGlobalNPC<RORGlobalNPC>().slowedBySpikestrip)
			{
				Vector2 drawCenter = new Vector2(npc.Center.X, npc.Top.Y + npc.gfxOffY - 20) - Main.screenPosition;
				Texture2D texture = ModContent.GetTexture("RiskOfSlimeRain/Textures/Slowdown");
				Rectangle destination = Utils.CenteredRectangle(drawCenter, texture.Size());
				destination.Inflate(10, 10);
				spriteBatch.Draw(texture, destination, drawColor);
			}
		}
	}
}
