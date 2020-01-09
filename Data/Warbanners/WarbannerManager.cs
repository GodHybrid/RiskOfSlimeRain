using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Projectiles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.World.Generation;
using WebmilioCommons.Tinq;

namespace RiskOfSlimeRain.Data.Warbanners
{
	/// <summary>
	/// Responsible for operating on warbanners and saving/loading them via RORWorld
	/// </summary>
	public static class WarbannerManager
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

		public static float WarbannerChance => MathHelper.Max((float)(0.04 * Math.Pow(Math.E, -0.2 * warbanners.Count)), 0.0002f);

		public static int Radius = -1;
		public static float X = 0f;
		public static float Y = 0f;

		public static float GetWarbannerCircleAlpha()
		{
			//0.3333f to 1f
			return (float)(Math.Sin((Main.GameUpdateCount / 8d) / (Math.PI * 2))) / 3f + 2/3f;
		}

		/// <summary>
		/// Checks for banner roll, and then adds a warbanner. Also syncs
		/// </summary>
		public static void TryAddWarbanner(int radius, Vector2 position)
		{
			if (Main.rand.NextFloat() < WarbannerChance)
			{
				//Find nearest solid tile below:
				while (!WorldUtils.Find(position.ToTileCoordinates(), Searches.Chain(new Searches.Down(1), new GenCondition[]
					{
						new Conditions.IsSolid()
					}), out _))
				{
					position.Y++;
				}
				position.Y -= 25; //half the projectiles height

				Radius = radius;
				X = position.X;
				Y = position.Y;
				new WarbannerPacket().Send();
				AddWarbanner();
			}
		}

		/// <summary>
		/// To add a new banner to the list, using the static Radius, X and Y variables
		/// </summary>
		public static void AddWarbanner()
		{
			if (Main.netMode == NetmodeID.MultiplayerClient) return;
			if (Radius == -1) return;
			if (warbanners.Count >= LIMIT)
			{
				warbanners.RemoveAt(0);
			}
			Warbanner banner = new Warbanner(Radius, new Vector2(X, Y));
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

			List<Warbanner> spawned = new List<Warbanner>();

			Main.player.DoActive(delegate (Player p)
			{
				spawned.Clear();
				for (int j = 0; j < unspawnedWarbanners.Count; j++)
				{
					Warbanner banner = unspawnedWarbanners[j];
					float distance = banner.radius + 1200; //1080 is width of the screen, add a bit of buffer
					float playerDistance = p.DistanceSQ(banner.position);
					if (playerDistance < distance * distance)
					{
						bool IsFresh = X == banner.position.X;
						Projectile.NewProjectile(banner.position, Vector2.Zero, ModContent.ProjectileType<WarbannerProj>(), 0, 0, Main.myPlayer, banner.radius, IsFresh.ToDirectionInt());
						spawned.Add(banner);
					}
				}

				for (int j = 0; j < spawned.Count; j++)
				{
					unspawnedWarbanners.Remove(spawned[j]);
				}
			});
		}

		/// <summary>
		/// Clears the warbanner list and also despawns all placed warbanners
		/// </summary>
		public static void Clear()
		{
			warbanners.Clear();
			Main.projectile.WhereActive(p => p.modProjectile is WarbannerProj).Do(p => p.Kill());
		}

		public static void Init()
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

		public static void Unload()
		{
			warbanners = unspawnedWarbanners = null;
		}

		public static void Save(TagCompound tag)
		{
			tag.Add("warbanners", warbanners);
		}
	}
}
