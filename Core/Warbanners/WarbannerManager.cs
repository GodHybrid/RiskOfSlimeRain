using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Network.Data;
using RiskOfSlimeRain.Projectiles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.WorldBuilding;

namespace RiskOfSlimeRain.Core.Warbanners
{
	/// <summary>
	/// Responsible for operating on warbanners and saving/loading them via RORWorld
	/// </summary>
	public class WarbannerManager : ModSystem
	{
		public const int LIMIT = 100;

		/// <summary>
		/// List to save and load warbanners
		/// </summary>
		public static List<Warbanner> warbanners;

		/// <summary>
		/// List to spawn warbanners
		/// </summary>
		public static List<Warbanner> unspawnedWarbanners;

		public static int KillCountForNextWarbanner => 30 + (int)(0.7f * Math.Pow(Math.Min(50, warbanners.Count), 2));

		public static float GetWarbannerCircleAlpha()
		{
			//0.666f to 1f
			return (float)Math.Sin((Main.GameUpdateCount / 8d) / (Math.PI * 2)) / 3f + 2 / 3f;
		}

		/// <summary>
		/// Adjusts banner position and then adds a warbanner if it's spawnable. Also syncs if successful. Returns true if adding warbanner was sucessful
		/// </summary>
		public static bool TryAddWarbanner(int radius, Vector2 position)
		{
			//Find nearest solid tile below:
			bool success = false;
			const int maxDistanceInTiles = WarbannerProj.Height / 16 + 3;
			if (FindNearestBelow(ref position, maxDistanceInTiles))
			{
				new WarbannerPacket(radius, position).Send();
				AddWarbanner(radius, position.X, position.Y);
				success = true;
			}
			return success;
		}

		private static bool FindNearestBelow(ref Vector2 position, int maxDistance)
		{
			bool success = false;
			Point p;
			if (WorldUtils.Find(position.ToTileCoordinates(), Searches.Chain(new Searches.Down(maxDistance), new GenCondition[]
				{
					new Conditions.IsSolid()
				}), out p))
			{
				position = p.ToWorldCoordinates(8f, 0f);
				position.Y -= WarbannerProj.Height >> 1; //Half the projectiles height
				success = true;
			}
			return success;
		}

		/// <summary>
		/// Add a new banner to the list
		/// </summary>
		public static void AddWarbanner(int radius, float x, float y)
		{
			if (Main.netMode == NetmodeID.MultiplayerClient) return;
			if (radius == -1) return;
			if (warbanners.Count >= LIMIT)
			{
				DeleteWarbanner(warbanners[0]);
			}
			Warbanner banner = new Warbanner(radius, x, y);
			warbanners.Add(banner);
			unspawnedWarbanners.Add(banner);
		}

		/// <summary>
		/// Goes through all players and checks if they are in range of any unspawned warbanners, then attempts to spawn them
		/// </summary>
		public static void TrySpawnWarbanners()
		{
			if (Main.netMode == NetmodeID.MultiplayerClient) return;
			if (unspawnedWarbanners.Count == 0) return;

			for (int i = 0; i < Main.maxPlayers; i++)
			{
				Player p = Main.player[i];

				if (p.active && !p.dead)
				{
					List<Warbanner> spawned = new List<Warbanner>();
					for (int j = 0; j < unspawnedWarbanners.Count; j++)
					{
						Warbanner banner = unspawnedWarbanners[j];
						float distance = banner.radius + 1200; //1080 is width of the screen, add a bit of buffer
						float playerDistance = p.DistanceSQ(banner.position);
						if (playerDistance < distance * distance)
						{
							//Adjust position in case tiles below it are gone if world is loaded again (means it will float if tiles are broken under an already spawned one)
							if (!banner.fresh)
							{
								int maxDistance = Math.Max(1, Main.maxTilesY - 45 - (int)(banner.position.Y / 16));
								FindNearestBelow(ref banner.position, maxDistance);
							}

							//Spawn banner projectile
							int whoami = Projectile.NewProjectile(new EntitySource_Misc("Warbanner"), banner.position, Vector2.Zero, ModContent.ProjectileType<WarbannerProj>(), 0, 0, Main.myPlayer, banner.radius, banner.fresh.ToDirectionInt());
							if (whoami < Main.maxProjectiles)
							{
								banner.associatedProjIdentity = Main.projectile[whoami].identity;
								spawned.Add(banner);
							}
						}
					}

					foreach (var banner in spawned)
					{
						unspawnedWarbanners.Remove(banner);
					}
				}
			}
		}

		/// <summary>
		/// Deletes the nearest warbanner to the player
		/// </summary>
		public static void DeleteNearestWarbanner(Player player)
		{
			//If client: since he has no backend data of warbanners, only the projectiles, it'll remove the projectile safely. The server then removes the banner too

			RORPlayer mPlayer = player.GetRORPlayer();
			if (mPlayer.InWarbannerRange)
			{
				int identity = mPlayer.LastWarbannerIdentity;
				if (identity > -1) //Maybe redundant check idk
				{
					DeleteWarbanner(identity);
				}
			}
		}

		/// <summary>
		/// Deletes the warbanner matching this identity from 'warbanners' and kills the projectile if it exists
		/// </summary>
		public static void DeleteWarbanner(int identity)
		{
			Warbanner banner = warbanners.Find(w => w.associatedProjIdentity == identity);
			if (banner != null)
			{
				DeleteWarbanner(banner);
			}
		}

		/// <summary>
		/// Deletes a given warbanner from the list and kills the projectile if it exists
		/// </summary>
		public static void DeleteWarbanner(Warbanner banner)
		{
			warbanners.Remove(banner);
			Projectile proj = FindWarbannerProj(banner.associatedProjIdentity);
			proj?.Kill();
		}

		/// <summary>
		/// Returns the warbanner projectile matching this identity, <see langword="null"/> if not found
		/// </summary>
		public static Projectile FindWarbannerProj(int identity)
		{
			for (int i = 0; i < Main.maxProjectiles; i++)
			{
				Projectile proj = Main.projectile[i];

				if (proj.active && proj.ModProjectile is WarbannerProj && proj.identity == identity)
				{
					return proj;
				}
			}
			return null;
		}

		/// <summary>
		/// Returns nearest warbanner projectile
		/// </summary>
		public static Projectile FindNearestWarbannerProj(Vector2 center)
		{
			float distanceSQ = float.MaxValue;
			Projectile proj = default(Projectile);
			for (int i = 0; i < Main.maxProjectiles; i++)
			{
				Projectile p = Main.projectile[i];
				if (p.active && p.ModProjectile is WarbannerProj)
				{
					float between = Vector2.DistanceSquared(center, p.Center);
					if (distanceSQ > between)
					{
						distanceSQ = between;
						proj = p;
					}
				}
			}
			return proj;
		}

		/// <summary>
		/// Returns nearest non-spawned warbanner
		/// </summary>
		public static Warbanner FindNearestInactiveWarbanner(Vector2 center)
		{
			float distanceSQ = float.MaxValue;
			Warbanner banner = default(Warbanner);
			foreach (Warbanner b in warbanners)
			{
				if (b.associatedProjIdentity == -1)
				{
					float between = Vector2.DistanceSquared(center, b.position);
					if (distanceSQ > between)
					{
						distanceSQ = between;
						banner = b;
					}
				}
			}
			return banner;
		}


		public override void ClearWorld()
		{
			warbanners = new List<Warbanner>();
			unspawnedWarbanners = new List<Warbanner>();
		}

		public static void Load(TagCompound tag)
		{
			warbanners.Clear();
			var list = tag.GetList<Warbanner>("warbanners");
			warbanners = (List<Warbanner>)list;
			unspawnedWarbanners = new List<Warbanner>(warbanners);
		}

		public override void OnModUnload()
		{
			warbanners = unspawnedWarbanners = null;
		}

		public static void Save(TagCompound tag)
		{
			tag.Add("warbanners", warbanners);
		}
	}
}
