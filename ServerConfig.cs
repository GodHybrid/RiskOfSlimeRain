using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace RiskOfSlimeRain
{
	public class ServerConfig : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ServerSide;

		public static ServerConfig Instance => ModContent.GetInstance<ServerConfig>();

		[Header("GameModes")]

		[ReloadRequired]
		[DefaultValue(false)]
		public bool OriginalStats;

		public const float TakenDamageMultiplier = 0.015f;

		public const float SpawnRateMultiplier = 0.02f;

		[ReloadRequired]
		[DefaultValue(true)]
		public bool DifficultyScaling;

		[Header("ServerTools")]

		[BackgroundColor(30, 30, 30)]
		public List<ItemDefinition> Blacklist = new List<ItemDefinition>();

		[ReloadRequired]
		[DefaultValue(false)]
		public bool DisableRecipes;

		[Header("Hint")]
		[JsonIgnore]
		[ShowDespiteJsonIgnore]
		public bool Hint => true;

		public static bool IsPlayerLocalServerOwner(int whoAmI)
		{
			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				return Netplay.Connection.Socket.GetRemoteAddress().IsLocalHost();
			}

			return NetMessage.DoesPlayerSlotCountAsAHost(whoAmI);
		}

		public override bool AcceptClientChanges(ModConfig pendingConfig, int whoAmI, ref NetworkText message)
		{
			if (Main.netMode == NetmodeID.SinglePlayer) return true;
			else if (!IsPlayerLocalServerOwner(whoAmI))
			{
				message = NetworkText.FromKey("tModLoader.ModConfigRejectChangesNotHost"); //"Only the host can change this config"
				return false;
			}
			return base.AcceptClientChanges(pendingConfig, whoAmI, ref message);
		}
	}
}
