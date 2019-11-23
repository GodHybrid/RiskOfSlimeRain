using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items
{
	class BustlingFungus : ModItem
	{

		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Grants \"Fungal Defense Mechanism\"\nStand still for 2 seconds to activate the buff\nHeals for 4.5% of your max HP every second");
		}

		public override void SetDefaults()
		{
			item.CloneDefaults(ItemID.LifeFruit);
			item.rare = ItemRarityID.White;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			TooltipLine line = new TooltipLine(mod, "Fungus", "The strongest biological healing agent...\n...is a mushroom");
			tooltips.Add(line);
			foreach (TooltipLine line2 in tooltips)
			{
				if (line2.Name == "Fungus")
				{
					line.overrideColor = Color.FloralWhite;
				}
			}
		}

		public override bool CanUseItem(Player player)
		{
			return true; // player.GetModPlayer<RORPlayer>().bitterRootIncrease < player.statLifeMax *3;
		}

		public override bool UseItem(Player player)
		{
			player.GetModPlayer<RORPlayer>().bustlingFungi++;
			player.GetModPlayer<RORPlayer>().bustlingFungusHeals++;
			player.GetModPlayer<RORPlayer>().fungalRadius += 16;
			player.GetModPlayer<RORPlayer>().fungalDefense = true;
			return true;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.GlowingMushroom, 500);
			recipe.AddIngredient(ItemID.RestorationPotion, 200);
			recipe.AddIngredient(ItemID.JungleSpores, 250);
			recipe.AddIngredient(ItemID.TealMushroom, 50);
			recipe.AddIngredient(ItemID.GreenMushroom, 50);
			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}
