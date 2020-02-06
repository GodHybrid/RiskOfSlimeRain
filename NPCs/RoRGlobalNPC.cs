using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RiskOfSlimeRain.Core.NPCEffects;
using System;
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

		public override void NPCLoot(NPC npc)
		{
			if (npc.boss)
			{
				int effectsPresent = 0;
				Main.player[(int)Player.FindClosest(npc.position, npc.width, npc.height)].GetModPlayer<RORPlayer>().Effects.ForEach(x => effectsPresent += x.Stack);
				if (Main.rand.NextFloat(1f) < (2f / (Math.Max(1, effectsPresent)))) Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("shit"), 1);
			}
		}
	}
}
