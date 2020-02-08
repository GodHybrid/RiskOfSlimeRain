using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RiskOfSlimeRain.Core.NPCEffects;
using RiskOfSlimeRain.Core.ROREffects;
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
			//Makeshift for now

			if (npc.boss)
			{
				RORRarity rarity = RORRarity.Common;
				//float rarityRand = Main.rand.NextFloat();
				//if (rarityRand < 0.05f)
				//{
				//	rarity = ROREffectRarity.Rare;
				//}
				//else if (rarityRand < 0.25f)
				//{
				//	rarity = ROREffectRarity.Uncommon;
				//}
				//else common

				List<int> items = ROREffectManager.GetItemTypesOfRarity(rarity);
				if (items.Count <= 0) return; //Item list empty, no items to drop! (mod is not complete yet)

				int itemType = Main.rand.Next(items);
				float chance = 2f / Math.Max(1, RORWorld.downedBossCount);
				if (Main.rand.NextFloat() < chance)
				{
					Item.NewItem(npc.getRect(), itemType, 1);
					RORWorld.downedBossCount++;
				}
			}
		}
	}
}
