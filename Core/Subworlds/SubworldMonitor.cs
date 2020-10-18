using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using RiskOfSlimeRain.Tiles.SubworldTiles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using WebmilioCommons.Tinq;

namespace RiskOfSlimeRain.Core.Subworlds
{
	public class SubworldMonitor
	{
		//TODO needs to carry over some things to a new monitor once the subworld changes to another one, and reset others
		public readonly string id = string.Empty;

		//All in ticks
		public readonly int teleporterTimeMax;

		public int TeleportReadyTimer { get; private set; }

		public bool TeleportReadyTimerDone { get; private set; }

		public int TicksSpentInSubworld { get; private set; }

		public bool TeleporterActivated { get; private set; }

		public bool TeleporterReady { get; private set; }

		public NPC NearestEnemy { get; private set; }

		public int EnemyCount { get; private set; }

		public bool TeleportInitiated { get; private set; }

		public int TeleportInitiateTimer { get; private set; }

		//TODO add some initial delay (wait for players money to convert)

		public bool DrawTeleport => TeleportInitiated && CanIncreaseTeleportInitiateTimer;

		public bool CanIncreaseTeleportInitiateTimer => TeleportInitiateTimer < teleportInitiateTimerMax;

		public bool TeleportReadyTimerRunning => TeleportReadyTimer < teleporterTimeMax;

		public const int teleportInitiateFrameDuration = 8;

		public const int teleportInitiateFrameCount = 4;

		/// <summary>
		/// The fade to black just before teleporting (number defined by the sound length)
		/// </summary>
		public const int teleportFadeOut = 112;

		public const int vanillaFadeInReduce = 25;

		public int BlackFadeInOverride => vanillaFadeInReduce + (int)(byte.MaxValue * (float)TeleportInitiateTimer / teleportFadeOut);

		/// <summary>
		/// Time it takes for the teleport draw sequence to finish
		/// </summary>
		public const int teleportInitiateTimerMax = teleportInitiateFrameDuration * teleportInitiateFrameCount;

		public int TeleportInitiateFrame => TeleportInitiateTimer / teleportInitiateFrameCount;

		public SoundEffectInstance TeleportSound { get; private set; }

		//Top left of teleporter tile
		private Point TeleporterPos { get; set; } = Point.Zero;

		public static bool HideLayers()
		{
			if (SubworldManager.Current == null) return false;

			return SubworldManager.Current.TeleportInitiated;
		}

		public static void DrawTeleportSequence(SpriteBatch spriteBatch, Player player)
		{
			if (SubworldManager.Current == null) return;

			if (!SubworldManager.Current.DrawTeleport) return;

			Texture2D image = ModContent.GetTexture("RiskOfSlimeRain/Textures/Recall");
			Rectangle bounds = new Rectangle
			{
				X = SubworldManager.Current.TeleportInitiateFrame,
				Y = 0,
				Width = image.Bounds.Width / teleportInitiateFrameCount,
				Height = image.Bounds.Height
			};
			bounds.X *= bounds.Width;

			spriteBatch.Draw(image, player.Bottom - Main.screenPosition - new Vector2(0, bounds.Height / 2), bounds, Color.White, 0f, bounds.Size() / 2, 1f, SpriteEffects.None, 0f);
		}

		public SubworldMonitor()
		{
			SubworldManager.Current = this;

			id = SubworldManager.GetActiveSubworldID();

			//90 * 60
			teleporterTimeMax = 2 * 60; //90 in normal/expert, 120 in master?
		}

		public bool GetGreetingText(out string displayName, out string subName, out float alpha)
		{
			displayName = subName = null;
			alpha = 0f;

			if (TicksSpentInSubworld > 360) return false;
			else if (TicksSpentInSubworld < 180)
			{
				alpha = 1f;
			}
			else
			{
				alpha = 2f - (TicksSpentInSubworld / 180f);
			}
			Subworld subworld = SubworldManager.subworlds[id];
			displayName = subworld.displayName;
			subName = subworld.subName;
			return alpha > 0f;
		}

		/// <summary>
		/// Returns the topleft tile position of a teleporter in the world
		/// </summary>
		public Point GetTeleporterPos()
		{
			if (TeleporterPos != Point.Zero) return TeleporterPos;

			for (int x = 42; x < Main.maxTilesX - 42; x++)
			{
				for (int y = 42; y < Main.maxTilesY - 42; y++)
				{
					Tile tile = Framing.GetTileSafely(x, y);
					if (tile.active() && tile.type == ModContent.TileType<TeleporterTile>())
					{
						TeleporterPos = new Point(x - tile.frameX / 18 % TeleporterTile.width, y - tile.frameY / 18 % TeleporterTile.height);
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

		//Call this when someone clicks the teleporter with sendToServer true
		public void InitiateTeleportSequence(bool sendToServer = false)
		{
			if (Main.netMode == NetmodeID.MultiplayerClient && sendToServer)
			{
				//TODO Send initiate packet to server
			}
			else
			{
				TeleportSound?.Stop();
				TeleportSound = Main.PlaySound(RiskOfSlimeRainMod.Instance.GetLegacySoundSlot(Terraria.ModLoader.SoundType.Custom, "Sounds/Custom/TeleporterRecall")?.WithVolume(0.8f));
				Main.OnPreDraw += ApplyBlackFadeIn;
				Main.LocalPlayer.AddBuff(BuffID.Webbed, teleportFadeOut + 60);
				TeleportInitiated = true;
			}
		}

		private void ApplyBlackFadeIn(GameTime _)
		{
			if (BlackFadeInOverride >= byte.MaxValue + vanillaFadeInReduce)
			{
				if (TeleportSound?.State != SoundState.Stopped)
				{
					TeleportSound.Stop();
					TeleportSound = null;
				}
				Main.OnPreDraw -= ApplyBlackFadeIn;
			}
			Main.BlackFadeIn = BlackFadeInOverride;
		}

		private bool TeleportEligible()
		{
			NearestEnemy = null;
			EnemyCount = 0;
			if (TeleportReadyTimerDone)
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
				if (TeleportReadyTimerRunning)
				{
					return $"{TeleportReadyTimer / 60 + 1}/{teleporterTimeMax / 60} seconds";
				}
				else if (TeleportReadyTimerDone && !TeleporterReady)
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
			//Each client and the server updates this on their own
			TicksSpentInSubworld++;

			if (TeleporterActivated && TeleportReadyTimerRunning)
			{
				TeleportReadyTimer++;
				if (!TeleportReadyTimerRunning)
				{
					//Last tick of timer, set done
					TeleportReadyTimerDone = true;
				}
			}

			if (TeleportReadyTimerDone && !TeleporterReady && Main.netMode != NetmodeID.MultiplayerClient)
			{
				if (TeleportEligible())
				{
					TeleporterReady = true;
					//TODO sync
				}
			}

			if (TeleportInitiated)
			{
				TeleportInitiateTimer++;
				if (TeleportInitiateTimer > teleportFadeOut)
				{
					if (Main.netMode != NetmodeID.MultiplayerClient)
					{
						SubworldManager.Exit();
					}
					//TODO change this so it works in multi
					SubworldManager.Current = null;
				}
			}
		}
	}
}
