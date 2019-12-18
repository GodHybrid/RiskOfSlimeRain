﻿using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Effects;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable
{
	public class Nullifier : ModItem
	{
		public sealed override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Resets all the upgrades you ever got");
		}

		public sealed override bool CanUseItem(Player player)
		{
			return player.GetModPlayer<RORPlayer>().Effects.Count > 0;
		}

		public sealed override bool UseItem(Player player)
		{
			RORPlayer mPlayer = player.GetModPlayer<RORPlayer>();
			mPlayer.Effects.Clear();
			ROREffectManager.Clear(mPlayer);
			return true;
		}

		public override void ModifyTooltips(List<TooltipLine> tooltips)
		{
			tooltips.Add(new TooltipLine(mod, Name, "Gone with the wind...")
			{
				overrideColor = Color.Red * (Main.mouseTextColor / 255f)
			});
		}

		public override void SetDefaults()
		{
			item.CloneDefaults(ItemID.LifeFruit);
			item.rare = ItemRarityID.Red;
			item.UseSound = new LegacySoundStyle(SoundID.Shatter, 0);
		}
	}
}