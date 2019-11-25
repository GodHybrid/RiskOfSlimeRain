using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items
{
	class StickyBomb : ModItem
	{
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("8% chance to attach a bomb to an enemy, detonating for 140% damage");
			DisplayName.SetDefault("Sticky Bomb");
		}

		public override void SetDefaults()
		{
			item.CloneDefaults(ItemID.LifeFruit);
			item.rare = ItemRarityID.White;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			TooltipLine line = new TooltipLine(mod, "Bomb", "Once you take the wrapping off, the adhesive is ACTIVE. DON'T TOUCH IT.\nYOU STICK THAT END ON BAD THINGS, NOT YOURSELF.");
			tooltips.Add(line);
			foreach (TooltipLine line2 in tooltips)
			{
				if (line2.Name == "Bomb")
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
			player.GetModPlayer<RORPlayer>().stickyBombs++;
			return true;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Bone, 999);
			recipe.AddIngredient(ItemID.Stinger, 400);
			recipe.AddIngredient(ItemID.SharkToothNecklace, 5);
			recipe.AddIngredient(ItemID.CoralstoneBlock, 55);
			recipe.AddIngredient(ItemID.SharkFin, 250);

			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}
