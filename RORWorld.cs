using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RiskOfSlimeRain.Core.Warbanners;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace RiskOfSlimeRain
{
	public class RORWorld : ModWorld
	{
		public override void Initialize()
		{
			WarbannerManager.Init();
		}

		public override void Load(TagCompound tag)
		{
			WarbannerManager.Load(tag);
		}

		public override TagCompound Save()
		{
			TagCompound tag = new TagCompound();
			WarbannerManager.Save(tag);
			return tag;
		}

		public override void PostUpdate()
		{
			WarbannerManager.TrySpawnWarbanners();
		}

		// A helper method that draws a bordered rectangle. 
		public static void DrawBorderedRect(SpriteBatch spriteBatch, Color color, Color borderColor, Vector2 position, Vector2 size, int borderWidth)
		{
			spriteBatch.Draw(Main.magicPixel, new Rectangle((int)position.X, (int)position.Y, (int)size.X, (int)size.Y), color);
			spriteBatch.Draw(Main.magicPixel, new Rectangle((int)position.X - borderWidth, (int)position.Y - borderWidth, (int)size.X + borderWidth * 2, borderWidth), borderColor);
			spriteBatch.Draw(Main.magicPixel, new Rectangle((int)position.X - borderWidth, (int)position.Y + (int)size.Y, (int)size.X + borderWidth * 2, borderWidth), borderColor);
			spriteBatch.Draw(Main.magicPixel, new Rectangle((int)position.X - borderWidth, (int)position.Y, (int)borderWidth, (int)size.Y), borderColor);
			spriteBatch.Draw(Main.magicPixel, new Rectangle((int)position.X + (int)size.X, (int)position.Y, (int)borderWidth, (int)size.Y), borderColor);
		}
	}
}
