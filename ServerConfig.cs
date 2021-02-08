using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace RiskOfSlimeRain
{
	[Label("Server Config")]
	public class ServerConfig : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ServerSide;

		public static ServerConfig Instance => ModContent.GetInstance<ServerConfig>();

		[Header("Game Modes" + "\n" +
			"You can only change those settings in the 'Mods' menu accessed from the main menu (not ingame)")]

		[Label("Original Stats")]
		[Tooltip("If enabled, replicates the item's stats from the original game for the most part. Leave it disabled for the recommended game balance")]
		[ReloadRequired]
		[DefaultValue(false)]
		public bool OriginalStats;

		public const float TakenDamageMultiplier = 0.01f;

		public const float SpawnRateMultiplier = 0.02f;

		[Label("Difficulty Scaling")]
		[Tooltip("If enabled, scales damage taken and enemy spawns by the amount of items that are currently active. Leave it enabled for the recommended game balance")]
		[ReloadRequired]
		[DefaultValue(true)]
		public bool DifficultyScaling;

		[Header("Server Tools")]

		[Label("Item Blacklist")]
		[Tooltip("Customize which items will have no effect on players (they can still apply it, it'll just stay inactive)")]
		[BackgroundColor(30, 30, 30)]
		public List<ItemDefinition> Blacklist = new List<ItemDefinition>();

		[Label("Disable Recipes")]
		[Tooltip("If enabled, disables recipes for the consumable power-up items")]
		[ReloadRequired]
		[DefaultValue(false)]
		public bool DisableRecipes;

		[Header("Hint: To go to the client config containing UI adjustments, press the '<' arrow in the bottom left")]
		[Label("Hint")]
		[JsonIgnore]
		public bool Hint => true;

		public static bool IsPlayerLocalServerOwner(int whoAmI)
		{
			if (Main.netMode == 1)
			{
				return Netplay.Connection.Socket.GetRemoteAddress().IsLocalHost();
			}

			for (int i = 0; i < Main.maxPlayers; i++)
			{
				RemoteClient client = Netplay.Clients[i];
				if (client.State == 10 && i == whoAmI && client.Socket.GetRemoteAddress().IsLocalHost())
				{
					return true;
				}
			}
			return false;
		}

		public override bool AcceptClientChanges(ModConfig pendingConfig, int whoAmI, ref string message)
		{
			if (Main.netMode == NetmodeID.SinglePlayer) return true;
			else if (!IsPlayerLocalServerOwner(whoAmI))
			{
				message = "You are not the server owner so you can not change this config";
				return false;
			}
			return base.AcceptClientChanges(pendingConfig, whoAmI, ref message);
		}
	}
}
