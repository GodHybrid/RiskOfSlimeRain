using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using WebmilioCommons.Tinq;

namespace RiskOfSlimeRain.Core.Subworlds
{
	public class SubworldMonitor
	{
		//TODO needs to carry over some things to a new monitor once the subworld changes to another one, and reset others
		//All in ticks
		public readonly int teleporterTimeMax;

		public int TeleporterTime { get; private set; }

		public int TicksSpentInSubworld { get; private set; }

		public bool TeleporterActivated { get; private set; }

		public bool TeleporterTimerDone { get; private set; }

		public bool TeleporterReady { get; private set; }

		public NPC NearestEnemy { get; private set; }

		public int EnemyCount { get; private set; }

		public bool TeleporterTimerRunning => TeleporterTime < teleporterTimeMax;

		//Top left of teleporter tile
		private Point TeleporterPos { get; set; } = Point.Zero;

		public SubworldMonitor()
		{
			SubworldManager.Current = this;
			//90 * 60
			teleporterTimeMax = 3 * 60; //90 in normal/expert, 120 in master?
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

		//Call this when someone clicks the teleporter with sendToServer true
		public void ActivateTeleporter(bool sendToServer = false)
		{
			if (Main.netMode == NetmodeID.MultiplayerClient && sendToServer)
			{
				//TODO Send activate packet to server
			}
			else
			{
				TeleporterActivated = true;
			}
		}

		private bool TeleportEligible()
		{
			NearestEnemy = null;
			EnemyCount = 0;
			if (TeleporterTimerDone)
			{
				var elig = Main.npc.WhereActive(n => n.CanBeChasedBy());
				NearestEnemy = elig.NearestActive(Main.LocalPlayer.Center, 1);
				EnemyCount = elig.Count;
				return EnemyCount == 0;
			}
			return false;
		}

		public string GetTeleporterTimerText()
		{
			if (TeleporterActivated)
			{
				if (TeleporterTimerRunning)
				{
					return $"{TeleporterTime / 60 + 1}/{teleporterTimeMax / 60} seconds";
				}
				else if (TeleporterTimerDone && !TeleporterReady)
				{
					if (EnemyCount > 0)
					{
						return $"Remaining Enemies: {EnemyCount}";
					}
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
				if (!TeleporterTimerRunning)
				{
					//Last tick of timer, set done
					TeleporterTimerDone = true;
				}
			}

			if (TeleporterTimerDone && !TeleporterReady && Main.netMode != NetmodeID.MultiplayerClient)
			{
				if (TeleportEligible())
				{
					TeleporterReady = true;
					//TODO sync
				}
			}
		}
	}
}
