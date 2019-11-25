using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items
{
	class Gasoline : ModItem
	{
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Killing enemies burns the ground to deal 60% damage and set enemies on fire");
		}

		public override void SetDefaults()
		{
			item.CloneDefaults(ItemID.LifeFruit);
			item.rare = ItemRarityID.White;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			TooltipLine line = new TooltipLine(mod, "Gasoline", "Gasoline, eh?\nSurprising to find a gas station these days, with everyone drivin' around them electro cars.");
			tooltips.Add(line);
			foreach (TooltipLine line2 in tooltips)
			{
				if (line2.Name == "Gasoline")
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
			player.GetModPlayer<RORPlayer>().gasCanisters++;
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
