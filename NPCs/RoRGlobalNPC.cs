using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RiskOfSlimeRain.Effects.Common;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.NPCs
{
	public class RORGlobalNPC : GlobalNPC
	{
		public override bool InstancePerEntity => true;

		public bool tasered;
		public bool slowedBySpikestrip;
		public sbyte bombTimer = 0;
		public sbyte frame = 0;
		public sbyte frameCounter = 0;

		public override void ResetEffects(NPC npc)
		{
			tasered = false;
			slowedBySpikestrip = false;
		}

		public override void AI(NPC npc)
		{
			if (tasered)
			{
				npc.velocity.X = 0;
				npc.position.X = npc.oldPosition.X;
				npc.velocity.Y = 0;
				npc.position.Y = npc.oldPosition.Y;
			}
			if (slowedBySpikestrip)
			{
				npc.velocity.X *= 0.9f;
			}
		}

		public override void DrawEffects(NPC npc, ref Color drawColor)
		{
			if (tasered)
			{
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
				}
				Lighting.AddLight(npc.position, 0.1f, 0.2f, 0.7f);
			}
		}

		public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Color drawColor)
		{
			//hardcoded for now
			SpikestripEffect.PostDraw(npc, spriteBatch, drawColor);
			TaserEffect.PostDraw(npc, spriteBatch, drawColor);
		}
	}
}
