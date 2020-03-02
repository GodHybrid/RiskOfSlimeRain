﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Items;
using RiskOfSlimeRain.Network.Data;
using RiskOfSlimeRain.Projectiles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.World.Generation;
using WebmilioCommons.Tinq;

namespace RiskOfSlimeRain.Core.Warbanners
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

		public static int KillCountForNextWarbanner => 30 + (int)(0.7f * Math.Pow(Math.Min(50, warbanners.Count), 2));

		public static float GetWarbannerCircleAlpha()
		{
			//0.666f to 1f
			return (float)Math.Sin((Main.GameUpdateCount / 8d) / (Math.PI * 2)) / 3f + 2 / 3f;
		}

		/// <summary>
		/// Adjusts banner position and then adds a warbanner. Also syncs
		/// </summary>
		public static void TryAddWarbanner(int radius, Vector2 position)
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

			new WarbannerPacket(radius, position).Send();
			AddWarbanner(radius, position.X, position.Y);
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

			Main.player.DoActive(delegate (Player p)
			{
				List<Warbanner> spawned = new List<Warbanner>();
				for (int j = 0; j < unspawnedWarbanners.Count; j++)
				{
					Warbanner banner = unspawnedWarbanners[j];
					float distance = banner.radius + 1200; //1080 is width of the screen, add a bit of buffer
					float playerDistance = p.DistanceSQ(banner.position);
					if (playerDistance < distance * distance)
					{
						int whoami = Projectile.NewProjectile(banner.position, Vector2.Zero, ModContent.ProjectileType<WarbannerProj>(), 0, 0, Main.myPlayer, banner.radius, banner.fresh.ToDirectionInt());
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
			});
		}

		public static void HighlightNearestWarbanner(SpriteBatch spriteBatch, Player player)
		{
			if (Main.netMode != NetmodeID.Server && player.whoAmI == Main.myPlayer && player.HeldItem.modItem is WarbannerRemover)
			{
				Projectile proj = FindWarbannerProj(player.GetRORPlayer().LastWarbannerIdentity);
				if (proj != null)
				{
					spriteBatch.Draw(Main.magicPixel, new Rectangle((int)(proj.position.X - Main.screenPosition.X + 2), (int)(proj.position.Y - Main.screenPosition.Y + 2), proj.width, proj.height), Color.OrangeRed * 0.5f);
				}
			}
		}

		/// <summary>
		/// Deletes the nearest warbanner to the player
		/// </summary>
		public static void DeleteNearestWarbanner(Player player)
		{
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
		private static Projectile FindWarbannerProj(int identity)
		{
			return Main.projectile.FirstActiveOrDefault(p => p.modProjectile is WarbannerProj && p.identity == identity);
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
