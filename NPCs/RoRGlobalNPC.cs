using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.NPCs
{
	public class RoRGlobalNPC : GlobalNPC
	{
		public override bool InstancePerEntity
		{
			get
			{
				return true;
			}
		}

		public bool tasered;
        public bool slowedBySpikestrip;

		public override void ResetEffects(NPC npc)
		{
            
		}

        public override void AI(NPC npc)
        {
            if (tasered)
            {
                npc.velocity.X *= 0;
                npc.position.X = npc.oldPosition.X;
            }
            if (slowedBySpikestrip)
            {
                npc.velocity.X *= 0.9f;
            }
            tasered = false;
            slowedBySpikestrip = false;
        }

        public override void SetDefaults(NPC npc)
		{
            
		}

		public override void NPCLoot(NPC npc)
		{
			
		}

		public override void DrawEffects(NPC npc, ref Color drawColor)
		{
			if (tasered)
			{
				if (Main.rand.Next(4) < 3)
				{
					int dust = Dust.NewDust(npc.position - new Vector2(2f, 2f), npc.width + 4, npc.height + 4, mod.DustType("EtherealFlame"), npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, default(Color), 3.5f);
					Main.dust[dust].noGravity = true;
					Main.dust[dust].velocity *= 1.8f;
					Main.dust[dust].velocity.Y -= 0.5f;
					if (Main.rand.Next(4) == 0)
					{
						Main.dust[dust].noGravity = false;
						Main.dust[dust].scale *= 0.5f;
					}
				}
				Lighting.AddLight(npc.position, 0.1f, 0.2f, 0.7f);
			}
		}

		public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
		{
			//if (player.GetModPlayer<ExamplePlayer>(mod).ZoneExample)
			//{
			//	spawnRate = (int)(spawnRate * 5f);
			//	maxSpawns = (int)(maxSpawns * 5f);
			//}
		}

		public override void SetupShop(int type, Chest shop, ref int nextSlot)
		{
			//if (type == NPCID.Dryad)
			//{
			//	shop.item[nextSlot].SetDefaults(mod.ItemType<Items.CarKey>());
			//	nextSlot++;

			//	shop.item[nextSlot].SetDefaults(mod.ItemType<Items.CarKey>());
			//	shop.item[nextSlot].shopCustomPrice = new int?(2);
			//	shop.item[nextSlot].shopSpecialCurrency = CustomCurrencyID.DefenderMedals;
			//	nextSlot++;

			//	shop.item[nextSlot].SetDefaults(mod.ItemType<Items.CarKey>());
			//	shop.item[nextSlot].shopCustomPrice = new int?(3);
			//	shop.item[nextSlot].shopSpecialCurrency = ExampleMod.FaceCustomCurrencyID;
			//	nextSlot++;
			//}
   //         else if (type == NPCID.Wizard && Main.expertMode)
   //         {
   //             shop.item[nextSlot].SetDefaults(mod.ItemType<Items.Infinity>());
   //             nextSlot++;
   //         }
		}

		// Make any NPC with a chat complain to the player if they have the stinky debuff.
		public override void GetChat(NPC npc, ref string chat)
		{
			//if (Main.LocalPlayer.HasBuff(BuffID.Stinky))
			//{
			//	switch (Main.rand.Next(3))
			//	{
			//		case 0:
			//			chat = "Eugh, you smell of rancid fish!";
			//			break;
			//		case 1:
			//			chat = "What's that horrid smell?!";
			//			break;
			//		default:
			//			chat = "Get away from me, i'm not doing any business with you.";
			//			break;
			//	}
			//}
		}

		// If the player clicks any chat button and has the stinky debuff, prevent the button from working.
		public override bool PreChatButtonClicked(NPC npc, bool firstButton)
		{
			return !Main.LocalPlayer.HasBuff(BuffID.Stinky);
		}
	}
}
