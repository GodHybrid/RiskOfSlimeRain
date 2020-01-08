using System.ComponentModel;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace RiskOfSlimeRain
{
	public class Config : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ClientSide;

		public static Config Instance => ModContent.GetInstance<Config>();

		[Label("Custom stacking")]
		[Tooltip("Manually select how many stacks an item has via scrollwheel (up to the unlocked amount)")]
		[DefaultValue(false)]
		public bool CustomStacking;

		//public override bool AcceptClientChanges(ModConfig pendingConfig, int whoAmI, ref string message)
		//{
		//	message = "Only the host of this world can change the config! Do so in singleplayer.";
		//	return false;
		//}
	}
}
