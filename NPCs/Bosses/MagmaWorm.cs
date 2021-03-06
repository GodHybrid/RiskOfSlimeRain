﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RiskOfSlimeRain.Core.ItemSpawning.NPCSpawning;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Items.Consumable.Boss;
using RiskOfSlimeRain.Network;
using RiskOfSlimeRain.Projectiles.Hostile;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using WebmilioCommons.Tinq;

namespace RiskOfSlimeRain.NPCs.Bosses
{
	//TODO MagmaWorm
	// Energized version
	public abstract class MagmaWorm : ModNPC
	{
		public const int defaultSize = 44;
		public const int positionOffset = 33;

		public virtual bool IsHead => this is MagmaWormHead;

		public virtual bool IsHeadExtension => this is MagmaWormHeadExtension;

		public virtual bool IsTail => this is MagmaWormTail;

		public virtual bool IsOtherBody(NPC other) => other.modNPC is MagmaWormBody || other.modNPC is MagmaWormTail;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Magma Worm");
			Main.npcFrameCount[npc.type] = 3;
		}

		public override void SetDefaults()
		{
			npc.Size = new Vector2(defaultSize);
			npc.aiStyle = -1;
			npc.netAlways = true;
			npc.damage = 56;
			npc.defense = 10;
			npc.lifeMax = 4000;
			npc.HitSound = SoundID.NPCHit7;
			npc.DeathSound = SoundID.NPCDeath8;
			npc.noGravity = true;
			npc.noTileCollide = true;
			npc.lavaImmune = true;
			//TODO remove when making it first level exclusive
			//npc.behindTiles = true;
			npc.knockBackResist = 0f;
			npc.value = Item.sellPrice(gold: 1);
			npc.scale = 1.4f;
			npc.buffImmune[BuffID.Poisoned] = true;
			npc.buffImmune[BuffID.OnFire] = true;
			npc.buffImmune[BuffID.CursedInferno] = true;
			npc.buffImmune[BuffID.Frostburn] = true;
			npc.boss = true;

			if (IsHead)
			{
				npc.npcSlots = 5f;
			}
			else
			{
				npc.defense = npc.defense << 1;
				npc.damage = npc.damage >> 1;
				npc.dontCountMe = true;
			}
		}

		public int ChildWhoAmI
		{
			get => (int)npc.ai[0];
			protected set => npc.ai[0] = value;
		}

		public int ParentWhoAmI
		{
			get => (int)npc.ai[1];
			protected set => npc.ai[1] = value;
		}

		public float Scale
		{
			get => npc.ai[2];
			private set => npc.ai[2] = value;
		}

		public int AttachedHealthWhoAmI
		{
			get => (int)npc.ai[3];
			protected set => npc.ai[3] = value;
		}

		public bool Synced
		{
			get => npc.localAI[0] == 1f;
			protected set => npc.localAI[0] = value ? 1f : 0f;
		}

		public bool SpawnedDead
		{
			get => npc.localAI[1] == 1f;
			protected set => npc.localAI[1] = value ? 1f : 0f;
		}

		public NPC Child => Main.npc[ChildWhoAmI];

		public NPC Parent => Main.npc[ParentWhoAmI];

		public Player Target => Main.player[npc.target];

		/// <summary>
		/// Handles spawning/despawning
		/// </summary>
		private void HandleSegments()
		{
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				if (IsHead && ChildWhoAmI == 0f)
				{
					AttachedHealthWhoAmI = npc.whoAmI;
					npc.realLife = npc.whoAmI;
					int parentWhoAmI = npc.whoAmI;
					int maxSegments = 23;
					float nextScaleStep = (npc.scale / maxSegments) * 0.60f; // Last segment will have 40% the starting scale
					float nextScale = npc.scale - nextScaleStep;

					// Spawn the hitbox extension
					int childWhoAmI = NPC.NewNPC((int)npc.Center.X, (int)npc.Bottom.Y, ModContent.NPCType<MagmaWormHeadExtension>(), npc.whoAmI);
					NPC childNPC = Main.npc[childWhoAmI];

					childNPC.realLife = npc.whoAmI;
					MagmaWorm childMW = childNPC.modNPC as MagmaWorm;
					childMW.AttachedHealthWhoAmI = npc.whoAmI;
					childMW.ParentWhoAmI = parentWhoAmI;
					childMW.Scale = nextScale;

					NetMessage.SendData(MessageID.SyncNPC, number: childWhoAmI);

					Scale = npc.scale;

					int body = ModContent.NPCType<MagmaWormBody>();
					int tail = ModContent.NPCType<MagmaWormTail>();
					for (int k = 0; k < maxSegments; k++)
					{
						int selectedType = body;
						if (k == maxSegments - 1)
						{
							selectedType = tail;
						}

						childWhoAmI = NPC.NewNPC((int)npc.Center.X, (int)npc.Bottom.Y, selectedType, npc.whoAmI);
						childNPC = Main.npc[childWhoAmI];

						childNPC.realLife = npc.whoAmI;
						childMW = childNPC.modNPC as MagmaWorm;
						childMW.AttachedHealthWhoAmI = npc.whoAmI;
						childMW.ParentWhoAmI = parentWhoAmI;
						childMW.Scale = nextScale;

						NPC parentNPC = Main.npc[parentWhoAmI];
						MagmaWorm parentMW = parentNPC.modNPC as MagmaWorm;
						parentMW.ChildWhoAmI = childWhoAmI;

						NetMessage.SendData(MessageID.SyncNPC, number: childWhoAmI);
						parentWhoAmI = childWhoAmI;
						nextScale -= nextScaleStep;
					}
				}

				if (!IsHead)
				{
					bool parentIsHead = (Parent.modNPC as MagmaWorm)?.IsHead ?? false;
					if (!Parent.active || !(IsOtherBody(Parent) || parentIsHead))
					{
						npc.life = 0;
						npc.HitEffect(0, 10.0);
						npc.active = false;
						NetMessage.SendData(MessageID.StrikeNPC, number: npc.whoAmI, number2: -1f);
					}
				}

				if (!(IsTail || IsHeadExtension))
				{
					bool childIsTail = (Parent.modNPC as MagmaWorm)?.IsTail ?? false;
					if (!Child.active || !(IsOtherBody(Child) || childIsTail))
					{
						npc.life = 0;
						npc.HitEffect(0, 10.0);
						npc.active = false;
						NetMessage.SendData(MessageID.StrikeNPC, number: npc.whoAmI, number2: -1f);
					}
				}

				if (!npc.active && Main.netMode == NetmodeID.Server)
				{
					NetMessage.SendData(MessageID.StrikeNPC, number: npc.whoAmI, number2: -1f);
				}
			}
		}

		/// <summary>
		/// Adjusts scale, realLife, targeting/despawn conditions
		/// </summary>
		private void TargetingAndEssentials()
		{
			if (AttachedHealthWhoAmI > 0)
			{
				npc.realLife = AttachedHealthWhoAmI;
			}

			if (npc.target < 0 || npc.target == 255 || Target.dead)
			{
				npc.TargetClosest(true);
			}

			if (Target.dead)
			{
				if (npc.timeLeft > 300)
				{
					npc.timeLeft = 300;
				}
			}

			npc.scale = Scale == 0 ? npc.scale : Scale;
		}

		public enum MWState : byte
		{
			Initializing,
			Diving,
			Disappearing,
			Emerging,
			SlowingDown,
		}

		/* // () are state transitions
		 * 
		 *                         Initializing
		 *                              |
		 *                              | (DisappearAfterInitializing)
		 *							    |
		 * (DisappearAfterDiving)       v     (EmergeAfterDisappearing)
		 *       ----------------> Disappearing -----------
		 *       |                                        |
		 *       |                                        v
		 *    Diving                                    Emerging
		 *       ^                                        |
		 *       |                                        |
		 *       ----------------SlowingDown <-------------
		 *  (DiveAfterSlowdown)                (SlowdownAfterEmerging)      
		 */

		public enum MWCommand : byte
		{
			/// <summary>
			/// Transitions into Disappearing, resets values
			/// </summary>
			Reset,
			/// <summary>
			/// Transitions into Diving
			/// </summary>
			Dive,
			/// <summary>
			/// Transitions into Disappearing
			/// </summary>
			Disappear,
			/// <summary>
			/// Transitions into Emerging, triggers warning
			/// </summary>
			Emerge,
			/// <summary>
			/// Transitions into SlowingDown
			/// </summary>
			Slowdown,
		}

		public class MagmaWormFSM : NPCFSM<MagmaWorm, MWState, MWCommand>
		{
			public MagmaWormFSM(MagmaWorm worm) : base(worm)
			{
				CurrentState = MWState.Initializing;

				// Define state transitions

				// Example: If current state is "Initializing", the command "Reset" will transition to the next state "Disappearing"
				var DisappearAfterInitializing = new StateTransition<MWState, MWCommand>(MWState.Initializing, MWCommand.Reset);

				// Regular state loop
				var DisappearAfterDiving = new StateTransition<MWState, MWCommand>(MWState.Diving, MWCommand.Disappear);
				var EmergeAfterDisappearing = new StateTransition<MWState, MWCommand>(MWState.Disappearing, MWCommand.Emerge);
				var SlowdownAfterEmerging = new StateTransition<MWState, MWCommand>(MWState.Emerging, MWCommand.Slowdown);
				var DiveAfterSlowingdown = new StateTransition<MWState, MWCommand>(MWState.SlowingDown, MWCommand.Dive);

				// Timeout transitions into Disappearing
				var DisappearAfterEmerging = new StateTransition<MWState, MWCommand>(MWState.Emerging, MWCommand.Reset);
				var DisappearAfterSlowingDown = new StateTransition<MWState, MWCommand>(MWState.SlowingDown, MWCommand.Reset);
				var DisappearAfterDiving2 = new StateTransition<MWState, MWCommand>(MWState.Diving, MWCommand.Reset);

				Transitions = new Dictionary<StateTransition<MWState, MWCommand>, MWState>
				{
					{ DisappearAfterInitializing, MWState.Disappearing },

					// Regular state loop
					{ DisappearAfterDiving, MWState.Disappearing },
					{ EmergeAfterDisappearing, MWState.Emerging },
					{ SlowdownAfterEmerging, MWState.SlowingDown },
					{ DiveAfterSlowingdown, MWState.Diving },

					// Timeout transitions into Disappearing
					{ DisappearAfterEmerging, MWState.Disappearing },
					{ DisappearAfterSlowingDown, MWState.Disappearing },
					{ DisappearAfterDiving2, MWState.Disappearing },
				};
			}

			private void Warning()
			{
				Me.EmergeWarning = true;
				Me.Location = Me.Target.Bottom;
				Main.PlaySound(SoundID.Roar, (int)Me.Location.X, (int)Me.Location.Y, 0, 0.8f);
			}

			//TODO proper watchdog timeout reset (so it runs even when state stays the same at invalid transition)
			private void Reset()
			{
				Me.EmergeWarning = false;
				Me.CurveDirection = 0;
				Me.WatchdogTimer = 0;
				Me.AITimer = 0;
			}

			private void DoEmerging()
			{
				Me.AITimer++;
				Vector2 direction = Me.Location - Me.npc.Center;
				int timerInvert = emergingTimerCurveMax - Me.AITimer;
				int rot = Utils.Clamp(timerInvert, 0, emergingTimerCurveMax);
				direction.Normalize();

				if (Me.CurveDirection == 0)
				{
					Me.CurveDirection = -Math.Sign(-direction.X);
				}

				direction = direction.RotatedBy(MathHelper.ToRadians((180f / emergingTimerCurveMax) * rot * Me.CurveDirection));
				float magnitude = 16f;
				direction *= 1f - (rot / (float)emergingTimerCurveMax);

				if (Me.AITimer > emergingTimerCurveMax)
				{
					magnitude = Math.Min(magnitude + 4 * Me.AITimer / (float)emergingTimerCurveMax, 32f);
				}

				Me.EmergeDirection = direction * magnitude;

				// For that nice initial curving
				float accel = Utils.Clamp(timerInvert, 20, emergingTimerCurveMax);
				Me.npc.velocity = (Me.npc.velocity * (accel - 1) + direction * magnitude) / accel;

				// Warning
				if (!Me.EmergeWarning && Me.npc.oldVelocity.Y >= 0 && Me.npc.velocity.Y < Me.npc.oldVelocity.Y)
				{
					Warning();
				}
			}

			private void DoSlowingDown()
			{
				Me.AITimer++;
				Me.npc.velocity *= 0.98f;
			}

			private void DoDiving()
			{
				Me.AITimer++;
				Vector2 direction = Me.Target.Center + Me.Target.velocity * 5f - Me.npc.Center;
				int timerInvert = divingTimerCurveMax - Me.AITimer;
				int rot = Utils.Clamp(timerInvert, 0, divingTimerCurveMax);
				direction.Normalize();

				if (Me.CurveDirection == 0)
				{
					Me.CurveDirection = Math.Sign(-direction.X);
				}

				float curveAmount = MathHelper.ToRadians((45f / divingTimerCurveMax) * rot * Me.CurveDirection);
				direction = direction.RotatedBy(curveAmount);
				float magnitude = 14f;

				Me.EmergeDirection = direction * magnitude;

				// For that nice initial curving
				float accel = Math.Max(divingTimerCurveMax - Me.AITimer / 2, 14f);
				Me.npc.velocity = (Me.npc.velocity * (accel - 1) + direction * magnitude) / accel;
			}

			private void DoDisappearing()
			{
				Me.AITimer++;
				Me.npc.velocity.X *= 0.97f;
				Me.npc.velocity.Y = Math.Min(Me.npc.velocity.Y + 0.3f, 18f);
			}

			public override void UpdateState()
			{
				if (Me.WatchdogTimer > watchdogTimerMax)
				{
					Reset();
					MoveNext(MWCommand.Reset); // Reset has a transition from every state
				}
				else if (Me.Target.dead && CurrentState != MWState.Disappearing)
				{
					Reset();
					MoveNext(MWCommand.Disappear); // Continue disappearing until timeLeft runs out
				}
				else
				{
					switch (CurrentState)
					{
						//TODO fix looping all states when about to emerge but still far below player
						case MWState.Disappearing:
							if (!Me.Target.dead && (Me.AITimer > disappearTimerMax || Me.npc.position.Y > Me.Target.BottomLeft.Y + 800))
							{
								// Go 800 coordinates below the player
								Reset();
								MoveNext(MWCommand.Emerge);
								if (Main.netMode != NetmodeID.MultiplayerClient)
								{
									Me.npc.netUpdate = true;
								}
							}
							break;
						case MWState.Emerging:
							if (Me.npc.Top.Y < Me.Location.Y)
							{
								Reset();
								//Main.NewText(Me.npc.velocity);
								MoveNext(MWCommand.Slowdown);
								Me.SpawnFireBalls();
							}
							break;
						case MWState.SlowingDown:
							//if (Me.npc.velocity.LengthSquared() < 4f)
							// TODO other condition here, make it similar to emerge but other way around
							//if (Me.npc.velocity.Y > 0)
							if (Me.AITimer > 24 || Me.npc.velocity.LengthSquared() <= 14f * 14f)
							{
								Reset();
								MoveNext(MWCommand.Dive);
							}
							break;
						case MWState.Diving:
							if (Me.AITimer > divingTimerMax)
							{
								Reset();
								MoveNext(MWCommand.Reset);
							}
							else if (Me.npc.Bottom.Y > Me.Target.Top.Y)
							{
								Reset();
								MoveNext(MWCommand.Disappear);
							}
							break;
						default: //Initializing falls under this
							Reset();
							MoveNext(MWCommand.Reset);
							break;
					}
				}
			}

			public override void ExecuteCurrentState()
			{
				switch (CurrentState)
				{
					case MWState.Disappearing:
						DoDisappearing();
						break;
					case MWState.Emerging:
						DoEmerging();
						break;
					case MWState.SlowingDown:
						DoSlowingDown();
						break;
					case MWState.Diving:
						DoDiving();
						break;
					default: //Initializing falls under this
						break;
				}
			}
		}

		public int AITimer { get; private set; }

		public const int disappearTimerMax = 180; // Disappear for 3 seconds after finished diving

		public const int emergingTimerCurveMax = 60; // Curving

		public const int divingTimerMax = 180; // Dive for 3 seconds max (if player for example drops down with slime mount, worm never catches up otherwise)

		public const int divingTimerCurveMax = 60; // Curving

		public const int slowingDownTimerMax = 120; // Reduce velocity for 2 seconds max, then start diving

		public int WatchdogTimer { get; private set; }

		public const int watchdogTimerMax = 300; // Force 5 second time max between stages

		public bool EmergeWarning { get; private set; }

		public Vector2 Location = Vector2.Zero;

		public int CurveDirection = 0;

		public Vector2 EmergeDirection = Vector2.Zero;

		private MagmaWormFSM _fsm;

		public MagmaWormFSM FSM
		{
			get
			{
				if (_fsm == null)
				{
					_fsm = new MagmaWormFSM(this);
				}
				return _fsm;
			}
		}

		public List<int> SpawnedFires { get; private set; } = new List<int>();

		public int frameOffset = -1;

		public int frame = 0;

		public static int shakeTimer = 0;

		public const int shakeTimerMax = 15;

		public virtual void HeadAI()
		{
			if (npc.velocity.X < 0f)
			{
				npc.spriteDirection = 1;
			}
			else if (npc.velocity.X > 0f)
			{
				npc.spriteDirection = -1;
			}

			FSM.UpdateState();
			FSM.ExecuteCurrentState();
			//Main.NewText(FSM.CurrentState);
			//Main.NewText(npc.velocity.Length());
			SpawnGroundFireAndDoScreenShake();

			npc.rotation = npc.velocity.ToRotation() + 1.57f;

			if (Main.netMode == NetmodeID.Server && !Synced)
			{
				npc.netUpdate = true;
				Synced = true;
			}
		}

		private bool TileAirOrNonSolid(Tile tile)
		{
			// If air, or if non-actuated and not a solid
			return !tile.nactive() || (tile.active() && !Main.tileSolid[tile.type]);
		}

		private bool TileSolid(Tile tile)
		{
			// If non-actuated and a solid or solid top
			return tile.nactive() && (Main.tileSolid[tile.type] || Main.tileSolidTop[tile.type]);
		}

		/// <summary>
		/// Checks the given tile position if it is suitable for a fire, then if it is, it spawns one and returns true
		/// </summary>
		private bool SpawnSuitableGroundFire(Point16 point, int type, bool actuallySpawnFire = true)
		{
			if (!WorldGen.InWorld(point.X, point.Y)) return false;
			Tile tile = Framing.GetTileSafely(point.X, point.Y);

			if (!WorldGen.InWorld(point.X, point.Y + 1)) return false;
			Tile tileBelow = Framing.GetTileSafely(point.X, point.Y + 1);

			if (TileAirOrNonSolid(tile) && TileSolid(tileBelow))
			{
				if (Main.netMode != NetmodeID.MultiplayerClient && actuallySpawnFire)
				{
					// NPC-owned projectiles spawn serverside
					Vector2 position = new Vector2(point.X * 16 + 8, point.Y * 16 + 8);

					// damageMultiplier is 2 if it's expert, 2 because hostile, another 2 because it shouldn't deal that much damage
					int damage = (int)(npc.damage / (Main.damageMultiplier * 2 * 2));

					Projectile.NewProjectile(position.X, position.Y, 0, 1, type, damage, 0, Main.myPlayer);
				}
				return true;
			}
			return false;
		}

		private void SpawnFires(Point16 start, int type, bool actuallySpawnFires = true)
		{
			Point16 point;
			// Left, center, and right of it
			for (int x = start.X - 1; x <= start.X + 1; x++)
			{
				point = new Point16(x, start.Y);
				if (!SpawnedFires.Contains(x))
				{
					bool spawned = SpawnSuitableGroundFire(point, type, actuallySpawnFires);
					if (spawned)
					{
						SpawnedFires.Add(point.X);
					}
				}
			}
		}

		public void SpawnGroundFireAndDoScreenShake(bool actuallySpawnFires = true)
		{
			float velocitySQ = npc.velocity.LengthSquared();

			if (velocitySQ < 5f * 5f) return; // Prevent fire spawn if moving slowly (only happens when manually spawned via cheatsheet)

			int type = ModContent.ProjectileType<FireProjHostile>();

			Point16 bottomCenter = npc.Bottom.ToTileCoordinates16();

			SpawnFires(bottomCenter, type, actuallySpawnFires);
			if (SpawnedFires.Count <= 0 && velocitySQ > 16 * 16)
			{
				// If the NPC moves so fast that it starts skipping tiles, try spawning fires again at a different place
				// This pretty much guarantees fires
				Point16 center = npc.Center.ToTileCoordinates16();
				SpawnFires(center, type, actuallySpawnFires);
			}

			if (SpawnedFires.Count > 0 && Main.netMode != NetmodeID.Server)
			{
				DoScreenShake(npc.Center);
			}

			SpawnedFires.Clear();
		}

		public void SpawnFireBalls()
		{
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				int amount = Math.Min((int)(2f * npc.lifeMax / npc.life), 10);

				if (Main.expertMode)
				{
					amount += 2;
				}

				float degrees = 5f;
				Vector2 direction = -Vector2.UnitY;

				float distanceX = Target.Center.X + Target.velocity.X - npc.Center.X;
				int sign = (distanceX > 0).ToDirectionInt();
				float tilt = 20 * Math.Min(1f, Math.Abs(distanceX) / 600);

				direction = direction.RotatedBy(MathHelper.ToRadians(sign * tilt));
				direction = direction.RotatedBy(-MathHelper.ToRadians(-degrees / 2 + degrees * amount / 2));
				int damage = (int)(npc.damage / (Main.damageMultiplier * 2 * 4));
				for (int i = 0; i < amount; i++)
				{
					Projectile.NewProjectile(npc.Top, direction * 10f, ModContent.ProjectileType<FireballGravityBouncy>(), damage, 0f, Main.myPlayer);
					direction = direction.RotatedBy(MathHelper.ToRadians(degrees));
				}
			}
		}

		private void BodyAI()
		{
			Vector2 pCenter = Parent.Center;
			float parentRotation = Parent.rotation;

			npc.velocity = Vector2.Zero;
			Vector2 newVelocity = pCenter - npc.Center;
			if (parentRotation != npc.rotation)
			{
				float rotatedBy = MathHelper.WrapAngle(parentRotation - npc.rotation);
				newVelocity = newVelocity.RotatedBy(rotatedBy * 0.1f);
			}

			if (IsHeadExtension)
			{
				// The extension points the other way
				newVelocity = -Parent.velocity;
			}

			npc.rotation = newVelocity.ToRotation() + 1.57f;

			// Rearrange position based on scale
			npc.position = npc.Center;
			npc.width = npc.height = (int)(defaultSize * npc.scale);
			npc.Center = npc.position;

			if (newVelocity != Vector2.Zero)
			{
				npc.Center = pCenter - Vector2.Normalize(newVelocity) * positionOffset * npc.scale;
			}
			npc.spriteDirection = (newVelocity.X > 0f) ? -1 : 1;
		}

		/// <summary>
		/// Runs the body or head AI
		/// </summary>
		private void AllAI()
		{
			if (!IsHead && ParentWhoAmI >= 0f && ParentWhoAmI < Main.maxNPCs)
			{
				if (Parent.modNPC is MagmaWorm mwb && mwb != null)
				{
					BodyAI();
				}
				else
				{
					npc.active = false; // Gets handled in HandleSegments
				}
			}
			else if (IsHead)
			{
				HeadAI();
			}
		}

		public sealed override void AI()
		{
			TargetingAndEssentials();

			HandleSegments();

			AllAI();
		}

		public override void HitEffect(int hitDirection, double damage)
		{
			if (npc.life > 0)
			{
				int i = 0;
				while (i < damage / npc.lifeMax * 50.0)
				{
					Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.Fire, 0f, 0f, 0, default(Color), 1.5f);
					dust.velocity *= 1.5f;
					dust.noGravity = true;
					i++;
				}
			}
			else
			{
				for (int i = 0; i < 10; i++)
				{
					Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.Fire, 0f, 0f, 0, default(Color), 1.5f);
					dust.velocity *= 2f;
					dust.noGravity = true;
				}
			}
		}

		public override void FindFrame(int frameHeight)
		{
			/* How it animates
			 * Loops from frame 0 to 2
			 * With frameOffset between 0 and 2
			 * 
			 * Hence frame can be between 0 and 2, 1 and 3, or 2 and 4
			 * Finally, it's modulo'd out to just between 0 and 2 for npc.frame.Y
			 */

			int count = Main.npcFrameCount[npc.type];

			if (frameOffset == -1)
			{
				int offset = npc.whoAmI % count;
				frameOffset = offset;
				frame = offset;
			}

			npc.frameCounter++;
			if (npc.frameCounter > 8)
			{
				npc.frameCounter = 0;
				frame++;
				if (frame >= count + frameOffset)
				{
					frame = frameOffset;
				}
			}

			npc.frame.Y = (frame % count) * frameHeight;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
		{
			if (!IsHead) return false;

			List<NPC> allNPCs = Main.npc.WhereActive(n => n.realLife == npc.whoAmI && !(n.modNPC is MagmaWormHeadExtension));
			//allNPCs.Reverse();

			foreach (NPC n in allNPCs)
			{
				float yoff = 56f * n.scale + 4f; // No clue why those magic numbers, 56 seems to be the distance between top pixel and first "body" pixel
				Texture2D texture = Main.npcTexture[n.type];
				Rectangle frame = n.frame;
				Vector2 drawOrigin = frame.Size() / 2;

				Vector2 drawPos = new Vector2(n.Center.X - Main.screenPosition.X - frame.Width * n.scale / 2f + drawOrigin.X * n.scale, n.position.Y - Main.screenPosition.Y + n.height - frame.Height * n.scale + drawOrigin.Y * n.scale + yoff);
				SpriteEffects spriteEffects = n.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
				spriteBatch.Draw(texture, drawPos, frame, n.GetAlpha(drawColor), n.rotation, drawOrigin, n.scale, spriteEffects, 0f);
			}

			return false;
		}

		public override Color? GetAlpha(Color drawColor)
		{
			return Color.White;
		}

		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			if (!IsHead) return false;
			// Only draws one healthbar per entire worm, hence only head

			Vector2 newPos = position;

			NPC tailNPC = Main.npc.FirstActiveOrDefault(n => n.modNPC is MagmaWormTail && n.realLife == npc.whoAmI);
			if (tailNPC != null)
			{
				// Calculate position between head and tail
				newPos += tailNPC.position;
				newPos /= 2f;

				position = newPos;
				scale = npc.scale;
			}
			return true;
		}

		public void GuaranteedNPCLoot()
		{
			Item.NewItem(npc.Hitbox, ItemID.Obsidian, Main.rand.Next(20, 26));
			Item.NewItem(npc.Hitbox, ItemID.Hellstone, Main.rand.Next(10, 14));
		}

		public override void NPCLoot()
		{
			//TODO redo this so it doesnt spawn items in terrain, but like destroyer does
			if (Main.netMode != NetmodeID.Server && Main.gameMenu && npc.Center == new Vector2(1000))
			{
				//Recipe browser
				Item.NewItem(npc.Hitbox, ModContent.ItemType<BurningWitness>());
				GuaranteedNPCLoot();
			}
			else
			{
				GuaranteedNPCLoot();

				//NPCLoot is only called on the head anyway: if (npc.realLife >= 0 && npc.realLife != npc.whoAmI)
				RORGlobalNPC.DropItemInstanced(npc, npc.position, npc.Hitbox.Size(), ModContent.ItemType<BurningWitness>(),
					npCondition: delegate (NPC n, Player player)
					{
						bool dropped = player.GetRORPlayer().burningWitnessDropped;
						if (dropped)
						{
							int random = NPCLootManager.RepeatedDropRate;
							if (Main.hardMode)
							{
								random = (int)(random * NPCLootManager.HMDropRateMultiplierForPreHMBosses);
							}

							return Main.rand.NextBool(random);
						}
						else return true;
					},
					onDropped: delegate (Player player, Item item)
					{
						player.GetRORPlayer().burningWitnessDropped = true;
						new BurningWitnessDroppedPacket(player.whoAmI).Send(toWho: player.whoAmI);
					}
				);

				if (!RORWorld.downedMagmaWorm)
				{
					RORWorld.downedMagmaWorm = true;
					if (Main.netMode == NetmodeID.Server)
					{
						new DownedMagmaWormPacket().Send();
					}
				}
			}
		}

		public override bool CheckDead()
		{
			if (!SpawnedDead)
			{
				SpawnedDead = true;
				npc.damage = 0;
				npc.life = npc.lifeMax;
				npc.dontTakeDamage = true;
				//npc.netUpdate = true;
				npc.NPCLoot();
				Main.PlaySound(SoundID.Roar, (int)npc.position.X, (int)npc.position.Y, 0, 1f, -0.18f);
				SpawnDead();

				return false;
			}
			return true;
		}

		private void SpawnDead()
		{
			List<NPC> allNPCs = Main.npc.WhereActive(n => n.realLife == npc.whoAmI && !(n.modNPC is MagmaWormHeadExtension));
			int newHead = ModContent.NPCType<MagmaWormDeadHead>();
			int newBody = ModContent.NPCType<MagmaWormDeadBody>();
			int newTail = ModContent.NPCType<MagmaWormDeadTail>();
			foreach (NPC n in allNPCs)
			{
				if (!(n.modNPC is MagmaWorm worm)) continue;

				int newType = worm.IsHead ? newHead : (worm.IsTail ? newTail : newBody);
				NPCHelper.Transform(n, newType, worm.Scale, true, true);
			}
		}

		/// <summary>
		/// Calculates the shakeTimer intensity based on distance to local player, and plays a sound
		/// </summary>
		public static void DoScreenShake(Vector2 position)
		{
			Main.PlaySound(SoundID.DD2_ExplosiveTrapExplode.SoundId, (int)position.X, (int)position.Y, SoundID.DD2_ExplosiveTrapExplode.Style, 0.5f, Main.rand.NextFloat(-0.6f, -0.4f));
			float lengthSQ = Vector2.DistanceSquared(Main.LocalPlayer.Center, position);
			float ratio = lengthSQ / (1080 * 1080);
			ratio = Utils.Clamp(ratio, 0f, 1f);
			shakeTimer = (int)((1f - ratio) * shakeTimerMax);
		}

		/// <summary>
		/// Decrements shakeTimer and does screen shake
		/// </summary>
		public static void UpdateScreenShake()
		{
			if (shakeTimer > 0)
			{
				shakeTimer--;
				Vector2 shake = new Vector2(Main.rand.NextFloat(shakeTimer), Main.rand.NextFloat(shakeTimer)) * 1.2f;
				Main.screenPosition += shake;
			}
		}
	}

	public class MagmaWormHead : MagmaWorm
	{

	}

	/// <summary>
	/// Additional NPC that serves as hitbox extension infront of the head
	/// </summary>
	public class MagmaWormHeadExtension : MagmaWorm
	{

	}

	public class MagmaWormBody : MagmaWorm
	{

	}

	public class MagmaWormTail : MagmaWorm
	{

	}
}
