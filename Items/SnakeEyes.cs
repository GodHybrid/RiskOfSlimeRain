using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items
{
	class SnakeEyes : ModItem
	{
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Increase crit chance by 6% for each time you're in peril, up to 4 times");
		}

		public override void SetDefaults()
		{
			item.CloneDefaults(ItemID.LifeFruit);
			item.rare = ItemRarityID.White;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			TooltipLine line = new TooltipLine(mod, "Dice", "You dirty ----------er\nYou KNEW I had to win to pay off my debts");
			tooltips.Add(line);
			foreach (TooltipLine line2 in tooltips)
			{
				if (line2.Name == "Dice")
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
			player.GetModPlayer<RORPlayer>().snakeEyesDice++;
			return true;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.RedCounterweight, 5);
			recipe.AddIngredient(ItemID.Ruby, 172);
			recipe.AddIngredient(ItemID.RichMahogany, 500);
			recipe.AddIngredient(ItemID.RedAcidDye, 10);
			recipe.AddIngredient(ItemID.SharkFin, 150);

			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}
