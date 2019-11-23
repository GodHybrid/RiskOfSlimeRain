using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items
{
	class HermitsScarf : ModItem
	{

		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Allows you to evade attacks with 10% chance");
		}

		public override void SetDefaults()
		{
			item.CloneDefaults(ItemID.LifeFruit);
			item.rare = ItemRarityID.White;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			TooltipLine line = new TooltipLine(mod, "Scarf", "This thing survived that horrible day\nIt must be able to survive whatever I have to endure, right?");
			tooltips.Add(line);
			foreach (TooltipLine line2 in tooltips)
			{
				if (line2.Name == "Scarf")
				{
					line.overrideColor = Color.FloralWhite;
				}
			}
		}

		public override bool CanUseItem(Player player)
		{
			if (player.GetModPlayer<RORPlayer>().scarfs >= 6) return false;
			return true;
		}

		public override bool UseItem(Player player)
		{
			player.GetModPlayer<RORPlayer>().scarfs++;
			return true;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Silk, 999);
			recipe.AddIngredient(ItemID.SlushBlock, 900);
			recipe.AddIngredient(ItemID.Feather, 350);
			recipe.AddIngredient(ItemID.Gi, 5);
			recipe.AddIngredient(ItemID.TrapsightPotion, 200);
			
			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}
