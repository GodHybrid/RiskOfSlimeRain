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
		[Tooltip("Manually select how many stacks an item has (up to the unlocked amount)")]
		[DefaultValue(false)]
		public bool CustomStacking;

		[Label("Only show when open inventory")]
		[Tooltip("Toggle if the UI should be always shown or only when the inventory is open")]
		[DefaultValue(false)]
		public bool OnlyShowWhenOpenInventory;

		[Label("Vertical offset")]
		[Tooltip("Adjust how far away the UI starts off vertically from the bottom")]
		[DefaultValue(0.05f)]
		[Range(0f, 1f)]
		public float ItemUIVerticalOffset;

		[Header("Visuals")]
		[Label("Hide own player visuals")]
		[Tooltip("Toggle visuals created by effects (with exceptions)")]
		[DefaultValue(false)]
		public bool HideOwnVisuals;

		[Label("Hide other player visuals")]
		[Tooltip("Toggle visuals created by effects (with exceptions)")]
		[DefaultValue(false)]
		public bool HideOtherVisuals;

		[Label("Hide warbanner radius")]
		[Tooltip("Toggle the warbanner circle")]
		[DefaultValue(false)]
		public bool HideWarbannerRadius;

		/// <summary>
		/// Check if visuals are hidden for this player (clientside)
		/// </summary>
		public static bool HiddenVisuals(Player player)
		{
			if (Main.myPlayer == player.whoAmI)
			{
				return Instance.HideOwnVisuals;
			}
			else
			{
				return Instance.HideOtherVisuals;
			}
		}

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
