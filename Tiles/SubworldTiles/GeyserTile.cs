using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RiskOfSlimeRain.Items.Placeable;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.ID;

namespace RiskOfSlimeRain.Tiles.SubworldTiles
{
	public class GeyserTile : DroppableTile<GeyserItem>
	{
		//"Hitbox"
		public const int height = 4;
		public const int width = 3;

		public override void SetDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = true;
			Main.tileWaterDeath[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x1);
			//Basically 3x1 which is the teleporter tile
			TileObjectData.newTile.Width = 3;
			TileObjectData.newTile.Height = 1;
			TileObjectData.newTile.Origin = new Point16(1, 0);
			TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
			TileObjectData.newTile.UsesCustomCanPlace = true;
			TileObjectData.newTile.CoordinateHeights = new int[] { 16 };
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(Type);
			ModTranslation name = CreateMapEntryName();
			soundType = SoundID.Tink;
			soundStyle = 2;
			name.SetDefault("Geyser");
			AddMapEntry(new Color(162, 184, 185), name);
		}

		/// <summary>
		/// Returns true if i and j match a geyser hitbox, and sets pos to the top-left tile coordinates of that geyser tile
		/// </summary>
		public static bool GetGeyserPos(int i, int j, out Point pos)
		{
			pos = Point.Zero;

			for (int y = 0; y < height; y++)
			{
				Tile tile = Framing.GetTileSafely(i, j + y);
				if (tile.active() && tile.type == ModContent.TileType<GeyserTile>())
				{
					TileObjectData data = TileObjectData.GetTileData(tile.type, 0);
					pos = new Point(i - tile.frameX / 18 % data.Width, j - tile.frameY / 18 % data.Height);
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Returns true if i and j match a geyser hitbox, and sets rect to the "hitbox" of the geyser
		/// </summary>
		public static bool GetGeyserHitbox(int i, int j, out Rectangle rect)
		{
			rect = Rectangle.Empty;
			if (GetGeyserPos(i, j, out Point pos))
			{
				rect = new Rectangle(pos.X * 16, (pos.Y + 1 - height) * 16, width * 16, height * 16);
				return true;
			}
			return false;
		}

		public static void Jump(Entity entity)
		{
			int jumpStrength = 50;
			switch (entity)
			{
				case Player player:
					player.fallStart = (int)(player.position.Y / 16f);
					player.jump = 0;
					//gravity is 0.4
					//check featherfall
					player.velocity.Y = -jumpStrength * player.gravity * (player.slowFall ? 1.78f : 1f);
					break;
				case NPC npc:
					//gravity is 0.3
					npc.velocity.Y = -jumpStrength * 0.345f;
					break;
				default:
					break;
			}
			Main.PlaySound(RiskOfSlimeRainMod.Instance.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/GeyserJump")?.WithVolume(.7f), entity.position);
		}

		public override bool CanPlace(int i, int j)
		{
			//Dunno if this is needed even
			return Main.tile[i, j + 1].active();
		}

		public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref Color drawColor, ref int nextSpecialDrawIndex)
		{
			Tile t = Main.tile[i, j];
			if (t.frameX == 0 && t.frameY == 0) //leftmost tile
			{
				Main.specX[nextSpecialDrawIndex] = i;
				Main.specY[nextSpecialDrawIndex] = j;
				nextSpecialDrawIndex++;
			}
		}

		uint oldUpdateCount = 0;
		int frameNum = 0;

		public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
		{
			const int frames = 6;
			const int height = 116;
			Vector2 pos = new Vector2(Main.offScreenRange);
			if (Main.drawToScreen)
			{
				pos = Vector2.Zero;
			}
			pos.X += i * 16 - (int)Main.screenPosition.X;//i*16 world coords left of block
			pos.Y += j * 16 + 16 - height + 2 - (int)Main.screenPosition.Y; //j*16 world coords top of block, +16 bottom of block, -height so draw can start at top left, +2 draw offset
			Texture2D animation = mod.GetTexture("Tiles/SubworldTiles/GeyserAnimation");
			int width = animation.Width / frames;

			if (oldUpdateCount != Main.GameUpdateCount)
			{
				frameNum = (frameNum + 1) % frames;
				oldUpdateCount = Main.GameUpdateCount;
			}

			Rectangle frame = new Rectangle(width * frameNum, 0, width, height);
			spriteBatch.Draw(animation, pos, frame, Color.White, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
		}

		public override void KillMultiTile(int i, int j, int frameX, int frameY)
		{
			if (frameX == 0 && frameY == 0)
			{
				Item.NewItem(i * 16, j * 16, 16, 16, ItemType);
			}
		}
	}
}
