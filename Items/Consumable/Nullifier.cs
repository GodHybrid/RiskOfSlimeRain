using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Helpers;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable
{
	public class Nullifier : ModItem
	{
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Use to enable the ability to restore your used '" + RiskOfSlimeRainMod.Instance.DisplayName + "' items, for a price");
		}

		public override bool CanUseItem(Player player)
		{
			return !player.GetRORPlayer().nullifierEnabled;
		}

		public override bool UseItem(Player player)
		{
			RORPlayer mPlayer = player.GetRORPlayer();
			mPlayer.nullifierEnabled = true;
			return true;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			if (Main.LocalPlayer.GetRORPlayer().nullifierEnabled)
			{
				tooltips.Add(new TooltipLine(mod, Name, "Nullifier is already enabled! Click the \"?\" in the UI"));
			}
			tooltips.Add(new TooltipLine(mod, Name, "Gone with the wind...")
			{
				overrideColor = Color.Red * (Main.mouseTextColor / 255f)
			});
		}

		public override void SetDefaults()
		{
			item.maxStack = 99;
			item.consumable = true;
			item.width = 18;
			item.height = 18;
			item.useStyle = 4;
			item.useTime = 30;
			item.useAnimation = 30;
			item.value = Item.sellPrice(0, 2, 0, 0);
			item.rare = ItemRarityID.Red;
			item.UseSound = new LegacySoundStyle(SoundID.Shatter, 0);
		}
	}
}
