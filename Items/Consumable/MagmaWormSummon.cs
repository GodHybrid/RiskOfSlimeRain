using Microsoft.Xna.Framework;
using RiskOfSlimeRain.NPCs.Bosses;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable
{
	public class MagmaWormSummon : ModItem
	{
		public static string tooltip = "Summons the Magma Worm";

		public override void SetStaticDefaults()
		{
			//TODO
			DisplayName.SetDefault("Spicy Honey Donut");
			Tooltip.SetDefault(tooltip);
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
			item.width = 28;
			item.height = 34;
			item.useStyle = ItemUseStyleID.HoldingUp;
			item.useTime = 30;
			item.useAnimation = 30;
			item.value = 0; //Ingredients cost nothing
			item.rare = ItemRarityID.Green;
			item.UseSound = new LegacySoundStyle(SoundID.MaxMana, 0);
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.LavaBucket, 4);
			recipe.AddIngredient(ItemID.CrispyHoneyBlock, 10);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}

		public override void OnCraft(Recipe recipe)
		{
			Item lavaBucket = recipe.requiredItem.FirstOrDefault(i => i.type == ItemID.LavaBucket);
			if (lavaBucket != null)
			{
				Main.LocalPlayer.QuickSpawnItem(ItemID.EmptyBucket, lavaBucket.stack);
			}
		}
	}
}
