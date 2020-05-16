using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.NPCs.Bosses;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable
{
	public class MagmaWormSummon : ModItem
	{
		public override void SetStaticDefaults()
		{
			//TODO
			DisplayName.SetDefault("Magma Worm Summon");
			Tooltip.SetDefault("Summons the Magma Worm");
		}

		public override bool UseItem(Player player)
		{
			Summon(player);
			return true;
		}

		public static void Summon(Player player)
		{
			if (Main.netMode == NetmodeID.MultiplayerClient) return;

			Vector2 position = player.Center + new Vector2(Main.rand.Next(-600, 600), 1000);
			int margin = 20 * 16;
			position.X = MathHelper.Clamp(position.X, margin, Main.maxTilesX * 16 - margin);
			position.Y = MathHelper.Clamp(position.Y, margin, Main.maxTilesY * 16 - margin);

			int whoami = NPC.NewNPC((int)position.X, (int)position.Y, ModContent.NPCType<MagmaWormHead>());

			if (Main.netMode == NetmodeID.Server && whoami < Main.maxNPCs)
			{
				NetMessage.SendData(MessageID.SyncNPC, number: whoami);
			}
		}

		public override void SetDefaults()
		{
			item.maxStack = 99;
			item.consumable = true;
			item.width = 32;
			item.height = 32;
			item.useStyle = ItemUseStyleID.HoldingUp;
			item.useTime = 30;
			item.useAnimation = 30;
			item.value = Item.sellPrice(0, 1, 0, 0);
			item.rare = ItemRarityID.Green;
			item.UseSound = new LegacySoundStyle(SoundID.MaxMana, 0);
		}
	}
}
