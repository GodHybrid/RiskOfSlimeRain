using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items
{
	class BitterRoot : ModItem
	{
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Permanently increases maximum life roughly by 8%");
		}

		public override void SetDefaults()
		{
			item.CloneDefaults(ItemID.LifeFruit);
			item.rare = ItemRarityID.White;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			TooltipLine line = new TooltipLine(mod, "Root", "Biggest. Ginseng. Root. Ever.");
			tooltips.Add(line);
			foreach (TooltipLine line2 in tooltips)
			{
				if (line2.Name == "Root")
				{
					line.overrideColor = Color.FloralWhite;
				}
			}
		}

		public override bool CanUseItem(Player player)
		{
			return player.GetModPlayer<RORPlayer>().bitterRootIncrease < player.statLifeMax * 3;
		}

		public override bool UseItem(Player player)
		{
			if (player.GetModPlayer<RORPlayer>().bitterRootIncrease + (int)((player.statLifeMax + player.GetModPlayer<RORPlayer>().bitterRootIncrease) * 0.08f) < player.statLifeMax * 3)
			{
				int increase = (int)((player.statLifeMax + player.GetModPlayer<RORPlayer>().bitterRootIncrease) * 0.08f);
				player.GetModPlayer<RORPlayer>().bitterRootIncrease += increase;
				player.statLifeMax2 += player.GetModPlayer<RORPlayer>().bitterRootIncrease;
				player.statLife += player.GetModPlayer<RORPlayer>().bitterRootIncrease;
				if (Main.myPlayer == player.whoAmI)
				{
					player.HealEffect(increase, true);
				}
			}
			else
			{
				//int increase = 10000 - player.GetModPlayer<RORPlayer>().bitterRootIncrease;
				int increase = (player.statLifeMax * 3) - player.GetModPlayer<RORPlayer>().bitterRootIncrease;
				player.GetModPlayer<RORPlayer>().bitterRootIncrease = (player.statLifeMax * 3);
				player.statLifeMax2 += player.GetModPlayer<RORPlayer>().bitterRootIncrease;
				player.statLife += player.GetModPlayer<RORPlayer>().bitterRootIncrease;
				if (Main.myPlayer == player.whoAmI)
				{
					player.HealEffect(increase, true);
				}
			}
			return true;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.LifeCrystal, 55);
			recipe.AddIngredient(ItemID.HealingPotion, 250);
			recipe.AddIngredient(ItemID.Salmon, 150);
			recipe.AddIngredient(ItemID.Blinkroot, 300);
			recipe.AddIngredient(ItemID.HoneyBlock, 400);
			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}
