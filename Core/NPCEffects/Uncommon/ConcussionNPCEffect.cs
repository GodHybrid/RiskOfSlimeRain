using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Core.NPCEffects.Uncommon
{
	public class ConcussionNPCEffect : NPCEffect
	{
		//public override void DrawEffects(NPC npc, ref Color drawColor)
		//{
		//	Lighting.AddLight(npc.position, 0.1f, 0.2f, 0.7f);
		//}

		public Vector2 oldVelocity = default;
		public int oldAIStyle = 0;
		public BitsByte oldDirections = 0;

		public override void Init(NPC npc)
		{
			oldVelocity = npc.velocity;
			oldAIStyle = npc.aiStyle;
			//oldDirections[0] = npc.direction > 0;
			//oldDirections[1] = npc.spriteDirection > 0;
		}

		public override void NetSend(BinaryWriter writer)
		{
			writer.WritePackedVector2(oldVelocity);
			writer.Write(oldDirections);
		}

		public override void NetReceive(BinaryReader reader)
		{
			oldVelocity = reader.ReadPackedVector2();
			oldDirections = reader.ReadByte();
		}

		public override void AI(NPC npc)
		{
			//npc.aiStyle = 0;
			float x = npc.velocity.X;
			float y = npc.velocity.Y;
			x /= 1.3f;
			y = y > 0.05f ? y / 1.1f : Math.Max(Math.Abs(y), 0.12f) * 1.4f;
			npc.velocity = new Vector2(x, y);

			//npc.position = npc.oldPosition;
			//npc.direction = oldDirections[0].ToDirectionInt();
			//npc.spriteDirection = oldDirections[1].ToDirectionInt();
		}

		public override void OnRemove(NPC npc)
		{
			npc.aiStyle = oldAIStyle;
			//npc.velocity = oldVelocity;
		}

		public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Color drawColor)
		{
			Vector2 drawCenter = new Vector2(npc.Center.X, npc.Top.Y + npc.gfxOffY - 20) - Main.screenPosition;
			Texture2D texture = ModContent.GetTexture("RiskOfSlimeRain/Textures/Stunned");
			Rectangle destination = Utils.CenteredRectangle(drawCenter, texture.Size());
			spriteBatch.Draw(texture, destination, Color.White);
		}
	}
}
