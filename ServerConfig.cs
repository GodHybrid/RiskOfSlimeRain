using System.ComponentModel;
using System.Runtime.Serialization;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace RiskOfSlimeRain
{
	[Label("Server Config")]
	public class ServerConfig : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ServerSide;

		public static ServerConfig Instance => ModContent.GetInstance<ServerConfig>();

		[Header("Game Modes")]

		[Label("Uhh")]
		[Tooltip("'Uhh': ")]
		[DefaultValue(false)]
		public bool Uhh;

		[OnDeserialized]
		internal void OnDeserializedMethod(StreamingContext context)
		{

		}
	}
}
