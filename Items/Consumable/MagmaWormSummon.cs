using Microsoft.Xna.Framework;
using RiskOfSlimeRain.NPCs.Bosses;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable
{
	public class MagmaWormSummon : ModItem
	{
		public override bool? UseItem(Player player)
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

			int whoami = NPC.NewNPC(NPC.GetBossSpawnSource(player.whoAmI), (int)position.X, (int)position.Y, ModContent.NPCType<MagmaWormHead>());

			if (Main.netMode == NetmodeID.Server && whoami < Main.maxNPCs)
			{
				NetMessage.SendData(MessageID.SyncNPC, number: whoami);
			}
		}

		public override void SetDefaults()
		{
			Item.maxStack = 9999;
			Item.consumable = true;
			Item.width = 28;
			Item.height = 34;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.value = 0; //Ingredients cost nothing
			Item.rare = ItemRarityID.Green;
			Item.UseSound = SoundID.MaxMana;
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.LavaBucket, 4);
			recipe.AddIngredient(ItemID.CrispyHoneyBlock, 10);
			recipe.DisableDecraft(); //Otherwise free lava buckets
			recipe.Register();
		}

		public override void OnCreated(ItemCreationContext context)
		{
			if (context is not RecipeItemCreationContext recipeContext)
			{
				return;
			}

			Item lavaBucket = recipeContext.ConsumedItems.FirstOrDefault(i => i.type == ItemID.LavaBucket);
			if (lavaBucket != null)
			{
				Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_FromThis(), ItemID.EmptyBucket, lavaBucket.stack);
			}
		}
	}
}
