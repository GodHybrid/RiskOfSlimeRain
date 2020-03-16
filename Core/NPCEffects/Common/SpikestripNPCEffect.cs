using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Core.NPCEffects.Common
{
	public class SpikestripNPCEffect : NPCEffect
	{
		public override void AI(NPC npc)
		{
			if (npc.velocity.X > 1f) npc.velocity.X *= 0.9f;
		}

		public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Color drawColor)
		{
			Vector2 drawCenter = new Vector2(npc.Center.X, npc.Top.Y + npc.gfxOffY - 20) - Main.screenPosition;
			Texture2D texture = ModContent.GetTexture("RiskOfSlimeRain/Textures/Slowdown");
			Rectangle destination = Utils.CenteredRectangle(drawCenter, texture.Size());
			destination.Inflate(10, 10);
			spriteBatch.Draw(texture, destination, Color.White);
		}
	}
}
