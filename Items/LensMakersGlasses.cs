using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items
{
	class LensMakersGlasses : ModItem
	{
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Increases crit chance by 10%");
		}

		public override void SetDefaults()
		{
			item.CloneDefaults(ItemID.LifeFruit);
			item.rare = ItemRarityID.White;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			TooltipLine line = new TooltipLine(mod, "Lens", "Calibrated for high focal alignment\nShould allow for the precision you were asking for");
			tooltips.Add(line);
			foreach (TooltipLine line2 in tooltips)
			{
				if (line2.Name == "Lens")
				{
					line.overrideColor = Color.FloralWhite;
				}
			}
		}

		public override bool CanUseItem(Player player)
		{
			if (player.GetModPlayer<RORPlayer>().lensMakersGlasses < 14) return true;
			return false;
		}

		public override bool UseItem(Player player)
		{
			player.GetModPlayer<RORPlayer>().lensMakersGlasses++;
			return true;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Lens, 500);
			recipe.AddIngredient(ItemID.BlackLens, 40);
			recipe.AddIngredient(ItemID.Ruby, 300);
			recipe.AddIngredient(ItemID.MeteoriteBrick, 350);
			recipe.AddIngredient(ItemID.CorruptHardenedSand, 250);

			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}
