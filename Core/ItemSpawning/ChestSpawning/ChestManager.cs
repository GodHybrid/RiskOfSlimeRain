using log4net;
using RiskOfSlimeRain.Core.ROREffects;
using RiskOfSlimeRain.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace RiskOfSlimeRain.Core.ItemSpawning.ChestSpawning
{
	/// <summary>
	/// Handles spawning ror items in chests on worldgen
	/// </summary>
	public static class ChestManager
	{
		/// <summary>
		/// Used during AddItemsToChests
		/// </summary>
		public static SortedSet<int> filledChests;

		/// <summary>
		/// Saved on the world, synced for "misc info"
		/// </summary>
		public static int totalChests = 0;

		public static bool IsGenerated => totalChests > 0;

		/// <summary>
		/// Returns the item type of a random consumable ror item
		/// </summary>
		public static int GetRandomRORItem(RORRarity rarity = RORRarity.Common)
		{
			return WorldGen.genRand.Next(ROREffectManager.GetItemTypesOfRarity(rarity));
		}

		/// <summary>
		/// Performs rng roll based on y height
		/// </summary>
		public static bool CanSpawnInChest(int y)
		{
			//TODO 1.4.4 dontdigup
			//40% chance of no item in a chest in the middle third of a world. 80% chance of no item on the surface and close to hell
			float noItem = 0.4f;
			//-200 is the "top" of hell, so add 100 more
			// rockLayer is where the dirt walls end, add 100 more
			if (y > Main.maxTilesY - 300 || y < Main.rockLayer + 100)
			{
				noItem = 0.8f;
			}
			return WorldGen.genRand.NextFloat() > noItem;
		}

		/// <summary>
		/// Checks if the tile is a chest of the specified "type" based on the position on the spritesheet
		/// </summary>
		public static bool IsChestOfType(Tile tile, int frameX)
		{
			return tile.TileType == TileID.Containers && tile.TileFrameX == frameX * 36;
		}

		/// <summary>
		/// Checks if the tile is a chest of one of the specified "types" based on the position on the spritesheet
		/// </summary>
		public static bool IsChestOfTypes(Tile tile, params int[] types)
		{
			return tile.TileType == TileID.Containers && Array.IndexOf(types, tile.TileFrameX / 36) > -1;
		}

		/// <summary>
		/// Spawns an item of specified type and stack in the chest of that index. Doesn't spawn an item if this chest already contains an item
		/// </summary>
		public static int PutItemIntoChest(int chestIndex, int type, int stack = 1)
		{
			if (filledChests.Contains(chestIndex)) return -1;
			Chest chest = Main.chest[chestIndex];
			if (chest == null) return -1;

			for (int i = 0; i < chest.item.Length; i++)
			{
				Item item = chest.item[i];
				if (item.type == 0 && type != 0 && stack > 0)
				{
					item.SetDefaults(type);
					item.stack = stack;
					filledChests.Add(chestIndex);
					return chestIndex;
				}
			}
			return -1;
		}

		/// <summary>
		/// Randomly selects one of the specified chests and calls <seealso cref="PutItemIntoChest"/>. Make sure to call this before other more general calls
		/// </summary>
		public static int PutItemInOneOfTheseChests(List<int> chests, int type, int stack = 1)
		{
			if (chests.Count > 0)
			{
				int pick = WorldGen.genRand.Next(chests);
				return PutItemIntoChest(pick, type, stack);
			}
			return -1;
		}

		/// <summary>
		/// Randomly selects a random chest that matches the predicate and puts a consumable ror item in it
		/// </summary>
		public static int PutRORItemInOneRandomChest(Func<Chest, Tile, bool> predicate, int stack = 1)
		{
			List<int> chests = FindAllChestsOf(delegate (Chest chest, Tile tile)
			{
				return predicate(chest, tile);
			});

			return PutItemInOneOfTheseChests(chests, GetRandomRORItem(), stack);
		}

		/// <summary>
		/// Returns a list of chest indices that matches the predicate
		/// </summary>
		public static List<int> FindAllChestsOf(Func<Chest, Tile, bool> predicate)
		{
			List<int> chests = new List<int>();
			for (int i = 0; i < Main.maxChests; i++)
			{
				Chest chest = Main.chest[i];
				if (chest == null) continue;

				Tile tile = Framing.GetTileSafely(chest.x, chest.y);

				if (chest.x < 42 || chest.x > Main.maxTilesX - 42) continue;
				if (chest.y < 42 || chest.y > Main.maxTilesY - 42) continue;

				bool success = predicate(chest, tile);
				if (success)
				{
					chests.Add(i);
				}
			}
			return chests;
		}

		public static void DoOceanChests()
		{
			const int oceanBorder = 300;
			PutRORItemInOneRandomChest(delegate (Chest chest, Tile tile)
			{
				return chest.y < Main.rockLayer && chest.x < oceanBorder && IsChestOfType(tile, ChestType.Water);
			});

			PutRORItemInOneRandomChest(delegate (Chest chest, Tile tile)
			{
				return chest.y < Main.rockLayer && chest.x > Main.maxTilesX - oceanBorder && IsChestOfType(tile, ChestType.Water);
			});
		}

		public static void DoPyramidChests()
		{
			PutRORItemInOneRandomChest(delegate (Chest chest, Tile tile)
			{
				return IsChestOfType(tile, ChestType.Gold) && tile.WallType == WallID.SandstoneBrick;
			});
		}

		public static void DoLivingWoodChests()
		{
			PutRORItemInOneRandomChest(delegate (Chest chest, Tile tile)
			{
				return IsChestOfType(tile, ChestType.LivingWood);
			});
		}

		public static void DoSkyChests()
		{
			PutRORItemInOneRandomChest(delegate (Chest chest, Tile tile)
			{
				return IsChestOfType(tile, ChestType.Sky);
			});
		}

		public static void DoPlanteraDungeonChests()
		{
			List<int> chests = FindAllChestsOf(delegate (Chest chest, Tile tile)
			{
				return IsChestOfTypes(tile,
					ChestType.DungeonJungleLocked,
					ChestType.DungeonCorruptionLocked,
					ChestType.DungeonCrimsonLocked,
					ChestType.DungeonHallowedLocked,
					ChestType.DungeonFrozenLocked);
			});

			foreach (var chest in chests)
			{
				PutItemIntoChest(chest, GetRandomRORItem(RORRarity.Uncommon));
			}
		}

		public static void DoOtherChests()
		{
			List<int> chests = FindAllChestsOf(delegate (Chest chest, Tile tile)
			{
				return IsChestOfTypes(tile,
					ChestType.Wood,
					ChestType.Gold,
					ChestType.GoldLocked,
					ChestType.Mahogany,
					ChestType.Ivy,
					ChestType.Ice,
					ChestType.Water,
					ChestType.Spider,
					ChestType.ShadowLocked,
					ChestType.Lihzard,
					ChestType.Marble,
					ChestType.Granite) && CanSpawnInChest(chest.y);
			});

			//otherChests now contains all chests that were rolled as eligible to receive an item

			foreach (var chest in chests)
			{
				PutItemIntoChest(chest, GetRandomRORItem());
			}

			//Count all chests in the world
			chests = FindAllChestsOf(delegate (Chest chest, Tile tile)
			{
				return tile.TileType == TileID.Containers;
			});
			totalChests = chests.Count;
		}

		private static string Warning(Exception e) => $"Error populating {0} chests: " + e.StackTrace + " " + e.Message;

		public static void AddItemsToChests()
		{
			ILog logger = RiskOfSlimeRainMod.Instance.Logger;
			filledChests = new SortedSet<int>();

			//Do guaranteed chests
			try { DoOceanChests(); }
			catch (Exception e)
			{
				logger.Warn(string.Format(Warning(e), "ocean"));
			}

			try { DoPyramidChests(); }
			catch (Exception e)
			{
				logger.Warn(string.Format(Warning(e), "pyramid"));
			}

			try { DoLivingWoodChests(); }
			catch (Exception e)
			{
				logger.Warn(string.Format(Warning(e), "living wood"));
			}

			try { DoSkyChests(); }
			catch (Exception e)
			{
				logger.Warn(string.Format(Warning(e), "sky"));
			}

			try { DoPlanteraDungeonChests(); }
			catch (Exception e)
			{
				logger.Warn(string.Format(Warning(e), "plantera dungeon"));
			}

			//Do all other chests
			try { DoOtherChests(); }
			catch (Exception e)
			{
				logger.Warn(string.Format(Warning(e), "all"));
			}

			logger.Info($"Populated {filledChests.Count}/{totalChests} = {(filledChests.Count / (float)totalChests).ToPercent()} of the worlds' chests!");

			filledChests = null;

			//Clear all chests of all ror items
			//for (int i = 0; i < Main.maxChests; i++)
			//{
			//	Chest chest = Main.chest[i];
			//	if (chest == null) continue;
			//	for (int j = 0; j < chest.item.Length; j++)
			//	{
			//		if (chest.item[j].modItem?.mod is RiskOfSlimeRainMod) chest.item[j].TurnToAir();
			//	}
			//}
		}

		public static void Init()
		{
			totalChests = 0;
		}

		public static void Load(TagCompound tag)
		{
			totalChests = tag.GetInt("totalChests");
		}

		public static void Save(TagCompound tag)
		{
			tag.Add("totalChests", totalChests);
		}

		internal static void NetSend(BinaryWriter writer)
		{
			writer.Write7BitEncodedInt(totalChests);
		}

		internal static void NetReceive(BinaryReader reader)
		{
			totalChests = reader.Read7BitEncodedInt();
		}
	}
}
