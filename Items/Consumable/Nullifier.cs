using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Helpers;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable
{
	public class Nullifier : ModItem
	{
		public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(Mod.DisplayName);

		public override void SetStaticDefaults()
		{
			// Tooltip.SetDefault("Use to enable the ability to restore your used '" + RiskOfSlimeRainMod.Instance.DisplayName + "' items, for a price");
		}

		public override bool CanUseItem(Player player)
		{
			return !player.GetRORPlayer().nullifierEnabled;
		}

		public override bool? UseItem(Player player)
		{
			RORPlayer mPlayer = player.GetRORPlayer();
			mPlayer.nullifierEnabled = true;
			return true;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			if (Main.LocalPlayer.HasItem(Item.type))
			{
				if (Main.LocalPlayer.GetRORPlayer().nullifierEnabled)
				{
					tooltips.Add(new TooltipLine(Mod, Name, "Nullifier is already enabled! Click the \"?\" in the UI"));
				}
			}
			else
			{
				int index = tooltips.FindLastIndex(t => t.Name.StartsWith("Tooltip"));
				if (index > -1)
				{
					tooltips.Insert(++index, new TooltipLine(Mod, Name, "25% chance to be sold by the Traveling Merchant in hardmode"));
				}
				else
				{
					tooltips.Add(new TooltipLine(Mod, Name, "25% chance to be sold by the Traveling Merchant in hardmode"));
				}
			}
			tooltips.Add(new TooltipLine(Mod, Name, "Gone with the wind...")
			{
				OverrideColor = Color.Red * (Main.mouseTextColor / 255f)
			});
		}

		public override void SetDefaults()
		{
			Item.maxStack = 9999;
			Item.consumable = true;
			Item.width = 18;
			Item.height = 18;
			Item.useStyle = 4;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.value = Item.sellPrice(0, 2, 0, 0);
			Item.rare = ItemRarityID.Red;
			Item.UseSound = SoundID.Shatter;
		}
	}
}
