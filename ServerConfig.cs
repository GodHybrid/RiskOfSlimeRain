using Newtonsoft.Json;
using System.ComponentModel;
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
		[Tooltip("If enabled, replicates the item's stats from the original game for the most part. Leave it disabled for the recommended game balance.")]
		//TODO debugging purposes
		//[ReloadRequired]
		[DefaultValue(false)]
		public bool RorStats;

		//TODO add scaling
		[Label("Difficulty Scaling")]
		[Tooltip("If enabled, scales damage taken and enemy spawns by the amount of items that are currently active. Leave it enabled for the recommended game balance.")]
		//TODO debugging purposes
		//[ReloadRequired]
		[DefaultValue(true)]
		public bool RorScaling;

		[Header("Hint: To go to the client config containing UI adjustments, press the '<' arrow in the bottom left")]
		[Label("Hint")]
		[JsonIgnore]
		public bool Hint => true;

		//[OnDeserialized]
		//internal void OnDeserializedMethod(StreamingContext context)
		//{

		//}
	}
}
