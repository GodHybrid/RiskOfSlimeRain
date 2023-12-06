using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects;
using RiskOfSlimeRain.Core.ROREffects.Common;
using RiskOfSlimeRain.Core.Warbanners;
using RiskOfSlimeRain.Helpers;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items
{
	public class WarbannerRemover : ModItem
	{
		public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(Mod.DisplayName);

		public override void SetDefaults()
		{
			Item.maxStack = 9999;
			Item.width = 18;
			Item.height = 18;
			Item.useStyle = 4;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.value = Item.buyPrice(gold: 15);
			Item.rare = ItemRarityID.Orange;
			Item.UseSound = SoundID.Item1;
		}

		public override bool CanUseItem(Player player)
		{
			return player.GetRORPlayer().InWarbannerRange;
		}

		public override bool? UseItem(Player player)
		{
			WarbannerManager.DeleteNearestWarbanner(player);

			//Local player resets their killcount if they have one
			if (Main.netMode != NetmodeID.Server)
			{
				WarbannerEffect effect = ROREffectManager.GetEffectOfType<WarbannerEffect>(Main.LocalPlayer);
				if (effect != null)
				{
					effect.ResetKillCount();
				}
			}
			return true;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			if (Main.LocalPlayer.HasItem(Item.type))
			{
				if (!Main.LocalPlayer.GetRORPlayer().InWarbannerRange)
				{
					tooltips.Add(new TooltipLine(Mod, Name, "You aren't standing inside the range of any warbanners!")
					{
						OverrideColor = Color.OrangeRed
					});
				}
			}
			else
			{
				int index = tooltips.FindLastIndex(t => t.Name.StartsWith("Tooltip"));
				TooltipLine line = new TooltipLine(Mod, Name, "25% chance to be sold by the Traveling Merchant after Skeletron has been defeated");
				if (index > -1)
				{
					tooltips.Insert(++index, line);
				}
				else
				{
					tooltips.Add(line);
				}
			}
		}
	}
}
