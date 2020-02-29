using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Core.NPCEffects.Common
{
	public class SpikestripNPCEffect : NPCEffect
	{
		public Vector2 oldVelocity = default;

		public override void Init(NPC npc)
		{
			oldVelocity = npc.velocity;
		}

		public override void NetSend(BinaryWriter writer)
		{
			writer.WritePackedVector2(oldVelocity);
		}

		public override void NetReceive(BinaryReader reader)
		{
			oldVelocity = reader.ReadPackedVector2();
		}

		public override void AI(NPC npc)
		{
			npc.velocity.X *= 0.9f;
		}

		public override void OnRemove(NPC npc)
		{
			npc.velocity = oldVelocity;
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
