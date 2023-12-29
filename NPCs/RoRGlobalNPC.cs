using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RiskOfSlimeRain.Core.ItemSpawning.NPCSpawning;
using RiskOfSlimeRain.Core.NPCEffects;
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

namespace RiskOfSlimeRain.NPCs
{
	public class RORGlobalNPC : GlobalNPC
	{
		public override bool InstancePerEntity => true;

		/// <summary>
		/// Serverside flag that indicates that this NPC was set to sync its SpawnedFromStatue bool
		/// </summary>
		public bool sentSpawnedFromStatue = false;

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

		public override void PostDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			foreach (var effect in NPCEffects) effect.PostDraw(npc, spriteBatch, drawColor);
		}

		public override void ModifyShop(NPCShop shop)
		{
			int type = shop.NpcType;
			if (type == NPCID.Painter)
			{
				shop.Add(ModContent.ItemType<ColossusPaintingItem>());
			}
		}

		public override void SetupTravelShop(int[] shop, ref int nextSlot)
		{
			if (NPC.downedBoss3 && Main.rand.NextBool(4)) //Skeletron
			{
				shop[nextSlot] = ModContent.ItemType<WarbannerRemover>();
				nextSlot++;
			}
			if (Main.hardMode && Main.rand.NextBool(4)) //Wof
			{
				shop[nextSlot] = ModContent.ItemType<Nullifier>();
				nextSlot++;
			}
		}

		public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
		{
			if (ServerConfig.Instance.DifficultyScaling)
			{
				float mult = player.GetRORPlayer().SpawnIncreaseMultiplier();
				spawnRate = (int)(spawnRate / mult);
				maxSpawns = (int)(maxSpawns * mult);
			}
		}

		public override void ModifyIncomingHit(NPC npc, ref NPC.HitModifiers modifiers)
		{
			if (modifiers.DamageType == ModContent.GetInstance<ArmorPenDamageClass>())
			{
				modifiers.ScalingArmorPenetration += 1f;
			}
		}

		public override void OnKill(NPC npc)
		{
			if (Main.netMode != NetmodeID.Server && Main.gameMenu) return; //RecipeBrowser protection

			NPCLootManager.DropItem(npc);

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
						new WarbannerRemoverDroppedPacket(player).Send(to: player.whoAmI);
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
				for (int i = 0; i < Main.maxPlayers; i++)
				{
					Player p = Main.player[i];

					if (p.active && !p.dead && (npc.playerInteraction[p.whoAmI] || !interactionRequired) && (npCondition?.Invoke(npc, p) ?? true))
					{
						int itemType = itemTypeFunc();
						int it = Item.NewItem(npc.GetSource_FromThis(), (int)Position.X, (int)Position.Y, (int)HitboxSize.X, (int)HitboxSize.Y, itemType, itemStack, true);
						Item item = Main.item[it];
						onDropped?.Invoke(p, item);
						Main.timeItemSlotCannotBeReusedFor[it] = 54000;
						NetMessage.SendData(MessageID.InstancedItem, p.whoAmI, -1, null, it);
						item.active = false;
						atleastOneDropped = true;
					}
				}
			}
			else if (Main.netMode == NetmodeID.SinglePlayer)
			{
				if (npCondition?.Invoke(npc, Main.LocalPlayer) ?? true)
				{
					int it = Item.NewItem(npc.GetSource_FromThis(), (int)Position.X, (int)Position.Y, (int)HitboxSize.X, (int)HitboxSize.Y, itemTypeFunc(), itemStack);
					Item item = Main.item[it];
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
