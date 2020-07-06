using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RiskOfSlimeRain.Core.ROREffects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WebmilioCommons.Tinq;

namespace RiskOfSlimeRain.Core.Subworlds
{
	public class SubworldMonitor
	{
		//All in ticks
		public readonly int teleporterTimeMax;

		public int TeleporterTime { get; private set; }

		public int TicksSpentInSubworld { get; private set; }

		public bool TeleporterActivated { get; private set; }

		public bool TeleporterReady { get; private set; }

		public bool TeleporterTimerRunning => TeleporterTime < teleporterTimeMax;
		//Top left of teleporter tile
		public Point TeleporterPos { get; private set; } = Point.Zero;

		public SubworldMonitor()
		{
			SubworldManager.Current = this;
			teleporterTimeMax = 90 * 60; //90 in normal/expert, 120 in master?
		}

		public Point GetTeleporterPos()
		{
			if (TeleporterPos != Point.Zero) return TeleporterPos;

			for (int x = 42; x < Main.maxTilesX - 42; x++)
			{
				for (int y = 42; y < Main.maxTilesY - 42; y++)
				{
					Tile tile = Framing.GetTileSafely(x, y);
					if (tile.active() && tile.type == TileID.Furnaces)
					{
						TeleporterPos = new Point(x - tile.frameX / 18 % 3, y - tile.frameY / 18 % 2);
						break;
					}
				}
				if (TeleporterPos != Point.Zero) break;
			}

			return TeleporterPos;
		}

		public void ActivateTeleporter()
		{
			TeleporterActivated = true;
		}

		public string GetTeleporterTimerText()
		{
			//int count = Main.npc.WhereActive(n => n.CanBeChasedBy()).Count;
			//if (count > 0)
			//	return $"Remaining Enemies: {count}";
			//else return null;
			if (TeleporterActivated)
			{
				if (TeleporterTimerRunning)
				{
					return $"{TeleporterTime / 60 + 1}/{teleporterTimeMax / 60} seconds";
				}
				else
				{
					int count = Main.npc.WhereActive(n => n.CanBeChasedBy()).Count;
					if (count > 0) return $"Remaining Enemies: {count}";
				}
			}
			return null;
		}

		public void Update()
		{
			TicksSpentInSubworld++;

			if (TeleporterActivated && TeleporterTimerRunning)
			{
				TeleporterTime++;
				if (!TeleporterTimerRunning && Main.netMode != NetmodeID.MultiplayerClient)
				{
					//Last tick of timer, set ready
					TeleporterReady = true;
					//TODO Send packet to all clients
				}
			}
		}


	}
}
