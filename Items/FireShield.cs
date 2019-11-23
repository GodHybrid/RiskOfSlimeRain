using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items
{
	class FireShield : ModItem
	{

		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("After being hit for 10% of your max health - explode, dealing 200 damage");
		}

		public override void SetDefaults()
		{
			item.CloneDefaults(ItemID.LifeFruit);
			item.rare = ItemRarityID.White;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			TooltipLine line = new TooltipLine(mod, "Shield", "The thing is only half-done, but it will do the job\nPLEASE handle with care!");
			tooltips.Add(line);
			foreach (TooltipLine line2 in tooltips)
			{
				if (line2.Name == "Shield")
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
			player.GetModPlayer<RORPlayer>().fireShields++;
			return true;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.ObsidianShield, 5);
			recipe.AddIngredient(ItemID.FlowerofFire, 3);
			recipe.AddIngredient(ItemID.HellstoneBrick, 666);
			recipe.AddIngredient(ItemID.Dynamite, 200);
			recipe.AddIngredient(ItemID.LargeAmber, 25);
			
			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}
