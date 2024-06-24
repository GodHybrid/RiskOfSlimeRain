using Terraria.ModLoader;

namespace RiskOfSlimeRain.MapLayers
{
	public static class MapLayerExtensions
	{
		//To mimic ModTexturedType.Texture
		public static string Texture(this ModMapLayer mapLayer)
		{
			return (mapLayer.GetType().Namespace + "." + mapLayer.GetType().Name).Replace('.', '/');
		}
	}
}
