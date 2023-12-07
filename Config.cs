using Newtonsoft.Json;
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

		[Header("ItemUI")]

		[DefaultValue(false)]
		public bool CustomStacking;

		[DefaultValue(false)]
		public bool OnlyShowWhenOpenInventory;

		[DefaultValue(0.05f)]
		[Range(0f, 1f)]
		public float ItemUIVerticalOffset;

		[Header("Visuals")]

		[DefaultValue(false)]
		public bool HideOwnVisuals;

		[DefaultValue(false)]
		public bool HideOtherVisuals;

		[DefaultValue(false)]
		public bool HideWarbannerRadius;

		[Header("Hint")]
		[JsonIgnore]
		[ShowDespiteJsonIgnore]
		public bool Hint => true;

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

		[OnDeserialized]
		internal void OnDeserializedMethod(StreamingContext context)
		{
			ItemUIVerticalOffset = Utils.Clamp(ItemUIVerticalOffset, 0f, 1f);
		}
	}
}
