using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RiskOfSlimeRain.Effects;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace RiskOfSlimeRain
{
	public static class RORInterfaceLayers
	{
		internal const int INVENTORY_SIZE = 47;

		public static string Name => ModContent.GetInstance<RiskOfSlimeRain>().Name;

		public static int hoverIndex = -1;

		internal static void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
		{
			if (Main.gameMenu) return;
			int inventoryIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));
			if (inventoryIndex != -1)
			{
				layers.Insert(inventoryIndex, new LegacyGameInterfaceLayer(
					$"{Name}: {nameof(Effects)}",
					Effects,
					InterfaceScaleType.UI
				));
			}
		}

		private static readonly GameInterfaceDrawMethod Effects = delegate
		{
			Player player = Main.LocalPlayer;
			List<ROREffect> effects = player.GetModPlayer<RORPlayer>().Effects;
			if (effects.Count == 0) return true;

			int xPosition = 0;
			int yPosition = 0;
			Rectangle sourceRect;
			Rectangle destRect;

			int numHorizontal = (Main.screenWidth - 3 * INVENTORY_SIZE) / 38;
			int lineOffset = 0;
			ROREffect effect;
			Texture2D texture;
			for (int i = 0; i < effects.Count; i++)
			{
				effect = effects[i];
				texture = ModContent.GetTexture(effect.Texture);

				lineOffset = i / numHorizontal;
				//2 * INVENTORY_SIZE is the distance needed to not overlap with recipe UI
				xPosition = 2 * INVENTORY_SIZE + (i - lineOffset * numHorizontal) * 38;

				yPosition = Main.screenHeight - 130 + lineOffset * 50;

				sourceRect = texture.Bounds;
				int width = sourceRect.Width;
				int height = sourceRect.Height;
				destRect = Utils.CenteredRectangle(new Vector2(xPosition, yPosition), new Vector2(width, height));
				Vector2 Bottom = new Vector2(xPosition, yPosition) + Main.screenPosition;
				//destRect.Y = (int)yPosition - height;
				if (i == 0)
				{
					//Utils.DrawLine(Main.spriteBatch, Bottom, new Vector2(Bottom.X + 500, Bottom.Y), Color.White, Color.White, 1);
				}
				//Utils.DrawLine(Main.spriteBatch, new Vector2(Center.X -1, Center.Y - 1), new Vector2(Center.X + 2, Center.Y + 2), Color.Green, Color.White, 4);

				Color color = Color.White * 0.8f;
				if (Config.Instance.CustomStacking && destRect.Contains(new Point(Main.mouseX, Main.mouseY)))
				{
					hoverIndex = i;
					destRect.Inflate(2, 2);
					color = Color.White;
				}

				Main.spriteBatch.Draw(texture, destRect, sourceRect, color);
				//Vector2 bottomCenter = destRect.BottomLeft();
				Vector2 bottomCenter = new Vector2(xPosition - width / 2, yPosition + 14);
				string text = "x" + effect.Stack.ToString();
				if (Config.Instance.CustomStacking && !effect.FullStack) text += "/" + effect.UnlockedStack;
				Vector2 length = Main.fontItemStack.MeasureString(text);
				bottomCenter.Y -= length.Y / 2;
				ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, Main.fontItemStack, text, bottomCenter, Color.White, 0, Vector2.Zero, Vector2.One * 0.78f);
			}
			return true;
		};
	}
}
