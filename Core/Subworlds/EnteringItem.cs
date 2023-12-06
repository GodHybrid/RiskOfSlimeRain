using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Chat;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Core.Subworlds
{
	//TODO add acquisition method
	/*
	public class EnteringItem : ModItem
	{
		public override string Texture => "Terraria/Images/Item_" + ItemID.Extractinator;

		public override void SetDefaults()
		{
			Item.maxStack = 1;
			Item.width = 34;
			Item.height = 38;
			Item.rare = 12;
			Item.useStyle = 4;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.UseSound = SoundID.Item1;
		}

		public override bool? UseItem(Player player)
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
						ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(message), Color.Orange);
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
	*/
}
