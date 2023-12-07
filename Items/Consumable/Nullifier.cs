using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Helpers;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable
{
	public class Nullifier : ModItem
	{
		public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(Mod.DisplayName);

		public static LocalizedText AlreadyEnabledText { get; private set; }
		public static LocalizedText ObtainmentText { get; private set; }
		public static LocalizedText FlavorText { get; private set; }

		public override void SetStaticDefaults()
		{
			AlreadyEnabledText = this.GetLocalization("AlreadyEnabled");
			ObtainmentText = this.GetLocalization("Obtainment");
			FlavorText = this.GetLocalization("Flavor");
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
					tooltips.Add(new TooltipLine(Mod, nameof(AlreadyEnabledText), AlreadyEnabledText.ToString()));
				}
			}
			else
			{
				int index = tooltips.FindLastIndex(t => t.Name.StartsWith("Tooltip"));
				var line = new TooltipLine(Mod, nameof(ObtainmentText), ObtainmentText.ToString());
				if (index > -1)
				{
					tooltips.Insert(++index, line);
				}
				else
				{
					tooltips.Add(line);
				}
			}
			tooltips.Add(new TooltipLine(Mod, nameof(FlavorText), FlavorText.ToString())
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
