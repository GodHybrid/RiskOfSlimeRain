using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects;
using RiskOfSlimeRain.Core.Warbanners;
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
		public sealed override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Resets all the upgrades you ever got");
		}

		public sealed override bool CanUseItem(Player player)
		{
			return player.GetRORPlayer().Effects.Count > 0 || WarbannerManager.warbanners.Count > 0;
		}

		public sealed override bool UseItem(Player player)
		{
			RORPlayer mPlayer = player.GetRORPlayer();
			//List<ROREffect> effects = new List<ROREffect>(mPlayer.Effects);
			mPlayer.Effects.Clear();
			ROREffectManager.Clear(mPlayer);
			WarbannerManager.Clear();
			//if (mPlayer.Effects.Count <= 0 && Main.netMode != NetmodeID.Server)
			//{
			//	foreach (ROREffect effect in effects)
			//	{
			//		int type = ROREffectManager.GetItemTypeOfEffect(effect);
			//		if (type >= 0)
			//		{
			//			player.QuickSpawnItem(type, effect.UnlockedStack);
			//		}
			//	}
			//}
			RORWorld.downedBossCount = 0;
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
