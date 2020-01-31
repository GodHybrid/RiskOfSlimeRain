using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RiskOfSlimeRain.Core.NPCEffects;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.NPCs
{
	public class RORGlobalNPC : GlobalNPC
	{
		public override bool InstancePerEntity => true;

		public List<NPCEffect> NPCEffects { get; set; } = new List<NPCEffect>();

		public override void ResetEffects(NPC npc)
		{
			NPCEffectManager.UpdateStatus(this);
		}

		public override void AI(NPC npc)
		{
			foreach (var effect in NPCEffects) effect.AI(npc);
		}

		public override void DrawEffects(NPC npc, ref Color drawColor)
		{
			foreach (var effect in NPCEffects) effect.DrawEffects(npc, ref drawColor);
		}

		public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Color drawColor)
		{
			foreach (var effect in NPCEffects) effect.PostDraw(npc, spriteBatch, drawColor);
		}
	}
}
