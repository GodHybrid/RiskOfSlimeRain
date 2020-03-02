using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects;
using RiskOfSlimeRain.Core.ROREffects.Common;
using RiskOfSlimeRain.Core.Warbanners;
using RiskOfSlimeRain.Helpers;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items
{
	public class WarbannerRemover : ModItem
	{
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Use to remove the nearest warbanner from '" + RiskOfSlimeRainMod.Instance.DisplayName + "'"
				+ "\nStand inside the range of a warbanner, and it will:"
				+ "\n- Highlight the warbanner that's about to be removed"
				+ "\n- Reset the killcount when used");
		}

		public override bool CanUseItem(Player player)
		{
			return player.GetRORPlayer().InWarbannerRange;
		}

		public override bool UseItem(Player player)
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
			if (!Main.LocalPlayer.GetRORPlayer().InWarbannerRange)
			{
				tooltips.Add(new TooltipLine(mod, Name, "You aren't standing inside the range of any warbanners!")
				{
					overrideColor = Color.OrangeRed
				});
			}
		}

		public override void SetDefaults()
		{
			item.maxStack = 99;
			item.width = 18;
			item.height = 18;
			item.useStyle = 4;
			item.useTime = 30;
			item.useAnimation = 30;
			item.value = Item.buyPrice(gold: 15);
			item.rare = ItemRarityID.Orange;
			item.UseSound = SoundID.Item1;
		}
	}
}
