using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects;
using RiskOfSlimeRain.Core.ROREffects.Common;
using RiskOfSlimeRain.Core.Warbanners;
using RiskOfSlimeRain.Helpers;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
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
