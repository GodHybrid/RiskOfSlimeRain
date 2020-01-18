using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Data.NPCEffects
{
	public class TaserNPCEffect : NPCEffect
	{
		public override void DrawEffects(NPC npc, ref Color drawColor)
		{
			//if (Main.rand.Next(4) < 3)
			//{
			//	//int dust = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, ModContent.DustType<EtherealFlame>(), npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, default(Color), 3.5f);
			//	//Main.dust[dust].noGravity = true;
			//	//Main.dust[dust].velocity *= 1.8f;
			//	//Main.dust[dust].velocity.Y -= 0.5f;
			//	//if (Main.rand.Next(4) == 0)
			//	//{
			//	//	Main.dust[dust].noGravity = false;
			//	//	Main.dust[dust].scale *= 0.5f;
			//	//}
			//}
			Lighting.AddLight(npc.position, 0.1f, 0.2f, 0.7f);
		}

		public override void AI(NPC npc)
		{
			npc.velocity = Vector2.Zero;
			npc.position = npc.oldPosition;
		}

		public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Color drawColor)
		{
			Vector2 drawCenter = new Vector2(npc.Center.X, npc.Top.Y + npc.gfxOffY - 20) - Main.screenPosition;
			Texture2D texture = ModContent.GetTexture("RiskOfSlimeRain/Textures/Tasered");
			Rectangle destination = Utils.CenteredRectangle(drawCenter, texture.Size());
			destination.Inflate(10, 0);
			spriteBatch.Draw(texture, destination, drawColor);
		}
	}
}
