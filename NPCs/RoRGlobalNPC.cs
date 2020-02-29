using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RiskOfSlimeRain.Core.NPCEffects;
using RiskOfSlimeRain.Core.ROREffects;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.NPCs
{
	public class RORGlobalNPC : GlobalNPC
	{
		public override bool InstancePerEntity => true;

		public List<NPCEffect> NPCEffects { get; set; } = new List<NPCEffect>();

		public override void ResetEffects(NPC npc)
		{
			NPCEffectManager.UpdateStatus(npc, this);
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

				//int itemType = Main.rand.Next(items);
				if (Main.rand.NextFloat() < RORWorld.DropChance)
				{
					int itemTypeFunc() => Main.rand.Next(items);
					DropItemInstanced(npc, npc.position, npc.Hitbox.Size(), itemTypeFunc);
					//Item.NewItem(npc.getRect(), itemType, 1);
					RORWorld.downedBossCount++;
				}
			}
		}

		/// <summary>
		/// Alternative, static version of npc.DropItemInstanced. Checks the playerCondition delegate before syncing/spawning the item
		/// </summary>
		public static void DropItemInstanced(NPC npc, Vector2 Position, Vector2 HitboxSize, Func<int> itemTypeFunc, int itemStack = 1, Func<NPC, Player, bool> condition = null, bool interactionRequired = true)
		{
			if (Main.netMode == NetmodeID.Server)
			{
				//Slightly modified from vanilla here: instead of spawning 1 item, then sending each player this item, spawn a new item and only send it to that player
				for (int p = 0; p < 255; p++)
				{
					if (Main.player[p].active && (npc.playerInteraction[p] || !interactionRequired))
					{
						if (condition != null && condition(npc, Main.player[p]) ||
							condition == null)
						{
							int itemType = itemTypeFunc();
							int item = Item.NewItem((int)Position.X, (int)Position.Y, (int)HitboxSize.X, (int)HitboxSize.Y, itemType, itemStack, true);
							Main.itemLockoutTime[item] = 54000;
							NetMessage.SendData(MessageID.InstancedItem, p, -1, null, item);
							Main.item[item].active = false;
						}
					}
				}
			}
			else if (Main.netMode == NetmodeID.SinglePlayer)
			{
				if (condition != null && condition(npc, Main.LocalPlayer) ||
					condition == null)
					Item.NewItem((int)Position.X, (int)Position.Y, (int)HitboxSize.X, (int)HitboxSize.Y, itemTypeFunc(), itemStack);
			}
		}

		public static void DropItemInstanced(NPC npc, Vector2 Position, Vector2 HitboxSize, int itemType, int itemStack = 1, Func<NPC, Player, bool> condition = null, bool interactionRequired = true)
		{
			DropItemInstanced(npc, Position, HitboxSize, () => itemType, itemStack, condition, interactionRequired);
		}
	}
}
