using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items
{
	class MysteriousVial : ModItem
	{

		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Permanently increases health regeneration by 1.2 health per second");
		}

		public override void SetDefaults()
		{
			item.CloneDefaults(ItemID.LifeFruit);
			item.rare = ItemRarityID.White;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			TooltipLine line = new TooltipLine(mod, "Vial", "Side effects may include itching, rashes, bleeding, sensitivity of skin,\ndry patches, permanent scarring, misaligned bone regrowth, rotting of the...");
			tooltips.Add(line);
			foreach (TooltipLine line2 in tooltips)
			{
				if (line2.Name == "Vial")
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
			player.GetModPlayer<RORPlayer>().mysteriousVials++;
			return true;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Bottle, 1);
			recipe.AddIngredient(ItemID.RegenerationPotion, 400);
			recipe.AddIngredient(ItemID.NeonTetra, 250);
			recipe.AddIngredient(ItemID.DaybloomSeeds, 600);
			recipe.AddIngredient(ItemID.Damselfish, 200);
			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}
