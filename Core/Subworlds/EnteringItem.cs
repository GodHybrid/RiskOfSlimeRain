﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.World.Generation;

namespace RiskOfSlimeRain.Core.Subworlds
{
	//TODO add acquisition method
	public class EnteringItem : ModItem
	{
		public override string Texture => "Terraria/Item_" + ItemID.Extractinator;

		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Use to enter a subworld. Only works with 'SubworldLibrary' Mod enabled");
		}

		public override void SetDefaults()
		{
			item.maxStack = 1;
			item.width = 34;
			item.height = 38;
			item.rare = 12;
			item.useStyle = 4;
			item.useTime = 30;
			item.useAnimation = 30;
			item.UseSound = SoundID.Item1;
		}

		public override bool UseItem(Player player)
		{
			//Enter should be called on exactly one side, which here is either the singleplayer player, or the server
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				bool result = SubworldManager.Enter(FirstLevelBasic.id) ?? false;

				if (!result)
				{
					//If some issue occured, inform why (can't know exactly obviously, might need to check logs)
					string message;
					if (!SubworldManager.Loaded)
					{
						message = "SubworldLibrary Mod is required to be enabled for this item to work!";
					}
					else
					{
						message = $"Unable to enter {FirstLevelBasic.id}!";
					}

					if (Main.netMode == NetmodeID.Server)
					{
						NetMessage.BroadcastChatMessage(NetworkText.FromLiteral(message), Color.Orange);
					}
					else
					{
						Main.NewText(message, Color.Orange);
					}
				}

				return result;
			}
			return true;
		}
	}
}
