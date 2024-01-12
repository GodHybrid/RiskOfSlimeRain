using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Core.NPCEffects.Common
{
	public class TaserNPCEffect : NPCEffect
	{
		public override void DrawEffects(NPC npc, ref Color drawColor)
		{
			Lighting.AddLight(npc.Center, 0.1f, 0.2f, 0.7f);
		}

		public Vector2 oldVelocity = default;
		public BitsByte oldDirections = 0;

		public override void Init(NPC npc)
		{
			oldVelocity = npc.velocity;
			oldDirections[0] = npc.direction > 0;
			oldDirections[1] = npc.spriteDirection > 0;
		}

		public override void NetSend(BinaryWriter writer)
		{
			writer.WriteVector2(oldVelocity);
			writer.Write(oldDirections);
		}

		public override void NetReceive(BinaryReader reader)
		{
			oldVelocity = reader.ReadVector2();
			oldDirections = reader.ReadByte();
		}

		public override void AI(NPC npc)
		{
			npc.velocity = Vector2.Zero;
			npc.position = npc.oldPosition;
			npc.direction = oldDirections[0].ToDirectionInt();
			npc.spriteDirection = oldDirections[1].ToDirectionInt();
		}

		public override void OnRemove(NPC npc)
		{
			npc.velocity = oldVelocity;
		}

		public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Color drawColor)
		{
			Vector2 drawCenter = new Vector2(npc.Center.X, npc.Top.Y + npc.gfxOffY - 20) - Main.screenPosition;
			Texture2D texture = ModContent.Request<Texture2D>("RiskOfSlimeRain/Textures/Tasered").Value;
			Rectangle destination = Utils.CenteredRectangle(drawCenter, texture.Size());
			spriteBatch.Draw(texture, destination, Color.White);
		}
	}
}
