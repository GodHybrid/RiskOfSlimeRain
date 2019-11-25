using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items
{
	class RustyKnife : ModItem
	{
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("15% chance to cause bleeding");
		}

		public override void SetDefaults()
		{
			item.CloneDefaults(ItemID.LifeFruit);
			item.rare = ItemRarityID.White;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			TooltipLine line = new TooltipLine(mod, "Knife", "Murder weapon, case name ELIAS. Probably a lover's spat?\nThere is still dried blood on the knife, so mark it as biological.");
			tooltips.Add(line);
			foreach (TooltipLine line2 in tooltips)
			{
				if (line2.Name == "Knife")
				{
					line.overrideColor = Color.FloralWhite;
				}
			}
		}

		public override bool CanUseItem(Player player)
		{
			if (player.GetModPlayer<RORPlayer>().rustyKnives < 7)
				return true;
			else return false;
		}

		public override bool UseItem(Player player)
		{
			player.GetModPlayer<RORPlayer>().rustyKnives++;
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
