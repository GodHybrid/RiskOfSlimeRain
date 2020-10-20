using Microsoft.Xna.Framework;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items
{
	class NoTextureItem : ModItem
	{
		public override string Texture => "RiskOfSlimeRain/Empty";

		public override void SetDefaults()
		{
			item.Size = new Vector2(16);
		}
	}
}
