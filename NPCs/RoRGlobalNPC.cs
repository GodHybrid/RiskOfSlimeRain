using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RiskOfSlimeRain.Core.NPCEffects;
using RiskOfSlimeRain.Core.ROREffects;
using RiskOfSlimeRain.Core.Warbanners;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Items;
using RiskOfSlimeRain.Items.Consumable;
using RiskOfSlimeRain.Items.Placeable.Paintings;
using RiskOfSlimeRain.Network;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WebmilioCommons.Tinq;

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

		public override void SetupShop(int type, Chest shop, ref int nextSlot)
		{
			if (type == NPCID.Painter)
			{
				shop.item[nextSlot].SetDefaults(ModContent.ItemType<ColossusPaintingItem>());
				nextSlot++;
			}
		}

		public override void SetupTravelShop(int[] shop, ref int nextSlot)
		{
			if (NPC.downedBoss3 && Main.rand.NextBool(4)) //Skeletron
			{
				shop[nextSlot] = ModContent.ItemType<WarbannerRemover>();
				nextSlot++;
			}
			else if(Main.hardMode && Main.rand.NextBool(4)) //Wof
			{
				shop[nextSlot] = ModContent.ItemType<Nullifier>();
				nextSlot++;
			}
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
					new DownedBossCountPacket().Send();
				}
			}

			if (npc.type == NPCID.SkeletronHead && WarbannerManager.warbanners.Count > 0)
			{
				DropItemInstanced(npc, npc.position, npc.Hitbox.Size(), ModContent.ItemType<WarbannerRemover>(),
					npCondition: delegate (NPC n, Player player)
					{
						return !player.GetRORPlayer().warbannerRemoverDropped;
					},
					onDropped: delegate (Player player, Item item)
					{
						player.GetRORPlayer().warbannerRemoverDropped = true;
						new WarbannerRemoverDroppedPacket(player.whoAmI).Send(toWho: player.whoAmI);
					}
				);
			}
			else if (npc.type == NPCID.WallofFlesh)
			{
				DropItemInstanced(npc, npc.position, npc.Hitbox.Size(), ModContent.ItemType<Nullifier>(),
					npCondition: delegate (NPC n, Player player)
					{
						RORPlayer mPlayer = player.GetRORPlayer();
						return !mPlayer.nullifierEnabled && mPlayer.Effects.Count > 0;
					}
				);
			}
		}

		/// <summary>
		/// Alternative, static version of npc.DropItemInstanced. Checks the npCondition delegate before syncing/spawning the item
		/// </summary>
		public static void DropItemInstanced(NPC npc, Vector2 Position, Vector2 HitboxSize, Func<int> itemTypeFunc, int itemStack = 1, Func<NPC, Player, bool> npCondition = null, Action<Player, Item> onDropped = null, Action afterAtleastOneDropped = null, bool interactionRequired = true)
		{
			bool atleastOneDropped = false;
			if (Main.netMode == NetmodeID.Server)
			{
				//Slightly modified from vanilla here: instead of spawning 1 item, then sending each player this item, spawn a new item and only send it to that player
				Main.player.WhereActive(p => (npc.playerInteraction[p.whoAmI] || !interactionRequired) && (npCondition?.Invoke(npc, p) ?? true)).Do(delegate (Player player)
				{
					int itemType = itemTypeFunc();
					int i = Item.NewItem((int)Position.X, (int)Position.Y, (int)HitboxSize.X, (int)HitboxSize.Y, itemType, itemStack, true);
					Item item = Main.item[i];
					onDropped?.Invoke(player, item);
					Main.itemLockoutTime[i] = 54000;
					NetMessage.SendData(MessageID.InstancedItem, player.whoAmI, -1, null, i);
					item.active = false;
					atleastOneDropped = true;
				});
			}
			else if (Main.netMode == NetmodeID.SinglePlayer)
			{
				if ((npCondition?.Invoke(npc, Main.LocalPlayer) ?? true) == true)
				{
					int i = Item.NewItem((int)Position.X, (int)Position.Y, (int)HitboxSize.X, (int)HitboxSize.Y, itemTypeFunc(), itemStack);
					Item item = Main.item[i];
					onDropped?.Invoke(Main.LocalPlayer, item);
					atleastOneDropped = true;
				}
			}

			if (atleastOneDropped)
			{
				afterAtleastOneDropped?.Invoke();
			}
		}

		public static void DropItemInstanced(NPC npc, Vector2 Position, Vector2 HitboxSize, int itemType, int itemStack = 1, Func<NPC, Player, bool> npCondition = null, Action<Player, Item> onDropped = null, Action afterAtleastOneDropped = null, bool interactionRequired = true)
		{
			DropItemInstanced(npc, Position, HitboxSize, () => itemType, itemStack, npCondition, onDropped, afterAtleastOneDropped, interactionRequired);
		}
	}
}
