using System.ComponentModel;
using System.Runtime.Serialization;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace RiskOfSlimeRain
{
	public class Config : ModConfig
	{
		public override ConfigScope Mode => ConfigScope.ClientSide;

		public static Config Instance => ModContent.GetInstance<Config>();

		[Header("Item UI")]

		[Label("Custom stacking")]
		[Tooltip("Manually select how many stacks an item has via scrollwheel (up to the unlocked amount)")]
		[DefaultValue(false)]
		public bool CustomStacking;

		[Label("Only show when open inventory")]
		[Tooltip("Toggle if the UI should be always shown or only when the inventory is open")]
		[DefaultValue(false)]
		public bool OnlyShowWhenOpenInventory;

		[Label("Item UI vertical offset")]
		[Tooltip("Adjust how far away the UI starts off vertically from the bottom")]
		[DefaultValue(0.05f)]
		[Range(0f, 1f)]
		public float ItemUIVerticalOffset;

		//public override bool AcceptClientChanges(ModConfig pendingConfig, int whoAmI, ref string message)
		//{
		//	message = "Only the host of this world can change the config! Do so in singleplayer.";
		//	return false;
		//}

		[OnDeserialized]
		internal void OnDeserializedMethod(StreamingContext context)
		{
			ItemUIVerticalOffset = Utils.Clamp(ItemUIVerticalOffset, 0f, 1f);
		}
	}
}
