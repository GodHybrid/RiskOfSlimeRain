using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items
{
	class LifeSavings : ModItem
	{
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Generate 1 copper every 3 seconds");
		}

		public override void SetDefaults()
		{
			item.CloneDefaults(ItemID.LifeFruit);
			item.rare = ItemRarityID.White;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			TooltipLine line = new TooltipLine(mod, "Savings", "hi im billy and heer is money for mom thanks");
			tooltips.Add(line);
			foreach (TooltipLine line2 in tooltips)
			{
				if (line2.Name == "Savings")
				{
					line.overrideColor = Color.FloralWhite;
				}
			}
		}

		public override bool CanUseItem(Player player)
		{
			return true;
		}

		public override bool UseItem(Player player)
		{
			player.GetModPlayer<RORPlayer>().savings++;
			player.GetModPlayer<RORPlayer>().piggyBankTimer = 180 / player.GetModPlayer<RORPlayer>().savings + 1;
			return true;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.PiggyBank, 5);
			recipe.AddIngredient(ItemID.GoldCoin, 150);
			recipe.AddIngredient(ItemID.ClayBlock, 600);
			recipe.AddIngredient(ItemID.GoldenCarp, 30);
			recipe.AddIngredient(ItemID.Bacon, 6);

			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}
