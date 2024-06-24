using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using RiskOfSlimeRain.Core.Warbanners;
using RiskOfSlimeRain.Items;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.UI;

namespace RiskOfSlimeRain.MapLayers
{
	internal class WarbannerMapLayer : ModMapLayerLocalized
	{
		private static Asset<Texture2D> texture;

		public static LocalizedText NameText { get; private set; }

		public override void Load()
		{
			texture = ModContent.Request<Texture2D>(this.Texture());

			NameText = this.GetLocalization("Name");
		}

		public override void Unload()
		{
			texture = null;
		}

		public override void Draw(ref MapOverlayDrawContext context, ref string text)
		{
			Player player = Main.LocalPlayer;
			if (WarbannerManager.warbanners.Count <= 0) return;
			if (player.HeldItem.ModItem is not WarbannerRemover) return;

			const float scaleIfNotSelected = 0.5f;
			const float scaleIfSelected = scaleIfNotSelected * 2f;
			foreach (var banner in WarbannerManager.warbanners)
			{
				//Get their proj position if possible
				Vector2 pos = banner.position / 16;
				int id = banner.associatedProjIdentity;
				if (id > -1)
				{
					Projectile proj = WarbannerManager.FindWarbannerProj(id);
					if (proj != null)
					{
						pos = proj.Center / 16;
					}
				}

				if (context.Draw(texture.Value, pos, Color.White, new SpriteFrame(1, 1, 0, 0), scaleIfNotSelected, scaleIfSelected, Alignment.Center).IsMouseOver)
				{
					text = NameText.ToString();
				}
			}
		}
	}
}
