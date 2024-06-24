using Terraria.ModLoader;

namespace RiskOfSlimeRain.MapLayers
{
	//Provides access to localization framework
	internal abstract class ModMapLayerLocalized : ModMapLayer, ILocalizedModType
	{
		public string LocalizationCategory => "MapLayers";
	}
}
