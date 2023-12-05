using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RiskOfSlimeRain.Core.ItemSpawning.NPCSpawning;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Items.Consumable.Boss;
using RiskOfSlimeRain.Network;
using RiskOfSlimeRain.Projectiles.Hostile;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.NPCs.Bosses
{
	//TODO 1.4.4 custom health bar
	//TODO MagmaWorm
	// Energized version
	public abstract class MagmaWorm : ModNPC
	{
		public const int defaultSize = 44;
		public const int positionOffset = 33;

		public virtual bool IsHead => this is MagmaWormHead;

		public virtual bool IsHeadExtension => this is MagmaWormHeadExtension;

		public virtual bool IsTail => this is MagmaWormTail;

		public virtual bool IsOtherBody(NPC other) => other.ModNPC is MagmaWormBody || other.ModNPC is MagmaWormTail;

		public static LocalizedText CommonNameText { get; private set; }

		public override LocalizedText DisplayName => CommonNameText;

		public override void SetStaticDefaults()
		{
			string category = $"NPCs.{nameof(MagmaWorm)}.";
			CommonNameText ??= Mod.GetLocalization($"{category}CommonName");
			// DisplayName.SetDefault("Magma Worm");
			Main.npcFrameCount[NPC.type] = 3;
		}

		public override void SetDefaults()
		{
			NPC.Size = new Vector2(defaultSize);
			NPC.aiStyle = -1;
			NPC.netAlways = true;
			NPC.damage = 56;
			NPC.defense = 10;
			NPC.lifeMax = 4000;
			NPC.HitSound = SoundID.NPCHit7;
			NPC.DeathSound = SoundID.NPCDeath8;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			NPC.lavaImmune = true;
			//TODO remove when making it first level exclusive
			//npc.behindTiles = true;
			NPC.knockBackResist = 0f;
			NPC.value = Item.sellPrice(gold: 1);
			NPC.scale = 1.4f;
			NPC.buffImmune[BuffID.Poisoned] = true;
			NPC.buffImmune[BuffID.OnFire] = true;
			NPC.buffImmune[BuffID.CursedInferno] = true;
			NPC.buffImmune[BuffID.Frostburn] = true;
			NPC.boss = true;

			if (IsHead)
			{
				NPC.npcSlots = 5f;
			}
			else
			{
				NPC.defense = NPC.defense << 1;
				NPC.damage = NPC.damage >> 1;
				NPC.dontCountMe = true;
			}
		}

		public int ChildWhoAmI
		{
			get => (int)NPC.ai[0];
			protected set => NPC.ai[0] = value;
		}

		public int ParentWhoAmI
		{
			get => (int)NPC.ai[1];
			protected set => NPC.ai[1] = value;
		}

		public float Scale
		{
			get => NPC.ai[2];
			private set => NPC.ai[2] = value;
		}

		public int AttachedHealthWhoAmI
		{
			get => (int)NPC.ai[3];
			protected set => NPC.ai[3] = value;
		}

		public bool Synced
		{
			get => NPC.localAI[0] == 1f;
			protected set => NPC.localAI[0] = value ? 1f : 0f;
		}

		public bool SpawnedDead
		{
			get => NPC.localAI[1] == 1f;
			protected set => NPC.localAI[1] = value ? 1f : 0f;
		}

		public NPC Child => Main.npc[ChildWhoAmI];

		public NPC Parent => Main.npc[ParentWhoAmI];

		public Player Target => Main.player[NPC.target];

		/// <summary>
		/// Handles spawning/despawning
		/// </summary>
		private void HandleSegments()
		{
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				if (IsHead && ChildWhoAmI == 0f)
				{
					AttachedHealthWhoAmI = NPC.whoAmI;
					NPC.realLife = NPC.whoAmI;
					int parentWhoAmI = NPC.whoAmI;
					int maxSegments = 23;
					float nextScaleStep = (NPC.scale / maxSegments) * 0.60f; // Last segment will have 40% the starting scale
					float nextScale = NPC.scale - nextScaleStep;

					// Spawn the hitbox extension
					int childWhoAmI = NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Bottom.Y, ModContent.NPCType<MagmaWormHeadExtension>(), NPC.whoAmI);
					NPC childNPC = Main.npc[childWhoAmI];

					childNPC.realLife = NPC.whoAmI;
					MagmaWorm childMW = childNPC.ModNPC as MagmaWorm;
					childMW.AttachedHealthWhoAmI = NPC.whoAmI;
					childMW.ParentWhoAmI = parentWhoAmI;
					childMW.Scale = nextScale;

					NetMessage.SendData(MessageID.SyncNPC, number: childWhoAmI);

					Scale = NPC.scale;

					int body = ModContent.NPCType<MagmaWormBody>();
					int tail = ModContent.NPCType<MagmaWormTail>();
					for (int k = 0; k < maxSegments; k++)
					{
						int selectedType = body;
						if (k == maxSegments - 1)
						{
							selectedType = tail;
						}

						childWhoAmI = NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Bottom.Y, selectedType, NPC.whoAmI);
						childNPC = Main.npc[childWhoAmI];

						childNPC.realLife = NPC.whoAmI;
						childMW = childNPC.ModNPC as MagmaWorm;
						childMW.AttachedHealthWhoAmI = NPC.whoAmI;
						childMW.ParentWhoAmI = parentWhoAmI;
						childMW.Scale = nextScale;

						NPC parentNPC = Main.npc[parentWhoAmI];
						MagmaWorm parentMW = parentNPC.ModNPC as MagmaWorm;
						parentMW.ChildWhoAmI = childWhoAmI;

						NetMessage.SendData(MessageID.SyncNPC, number: childWhoAmI);
						parentWhoAmI = childWhoAmI;
						nextScale -= nextScaleStep;
					}
				}

				if (!IsHead)
				{
					bool parentIsHead = (Parent.ModNPC as MagmaWorm)?.IsHead ?? false;
					if (!Parent.active || !(IsOtherBody(Parent) || parentIsHead))
					{
						NPC.life = 0;
						NPC.HitEffect(0, 10.0);
						NPC.active = false;
						NetMessage.SendData(MessageID.DamageNPC, number: NPC.whoAmI, number2: -1f);
					}
				}

				if (!(IsTail || IsHeadExtension))
				{
					bool childIsTail = (Parent.ModNPC as MagmaWorm)?.IsTail ?? false;
					if (!Child.active || !(IsOtherBody(Child) || childIsTail))
					{
						NPC.life = 0;
						NPC.HitEffect(0, 10.0);
						NPC.active = false;
						NetMessage.SendData(MessageID.DamageNPC, number: NPC.whoAmI, number2: -1f);
					}
				}

				if (!NPC.active && Main.netMode == NetmodeID.Server)
				{
					NetMessage.SendData(MessageID.DamageNPC, number: NPC.whoAmI, number2: -1f);
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
				NPC.realLife = AttachedHealthWhoAmI;
			}

			if (NPC.target < 0 || NPC.target == 255 || Target.dead)
			{
				NPC.TargetClosest(true);
			}

			if (Target.dead)
			{
				if (NPC.timeLeft > 300)
				{
					NPC.timeLeft = 300;
				}
			}

			NPC.scale = Scale == 0 ? NPC.scale : Scale;
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
				SoundEngine.PlaySound(SoundID.Roar.WithVolumeScale(0.8f), Me.Location);
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
				Vector2 direction = Me.Location - Me.NPC.Center;
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
				Me.NPC.velocity = (Me.NPC.velocity * (accel - 1) + direction * magnitude) / accel;

				// Warning
				if (!Me.EmergeWarning && Me.NPC.oldVelocity.Y >= 0 && Me.NPC.velocity.Y < Me.NPC.oldVelocity.Y)
				{
					Warning();
				}
			}

			private void DoSlowingDown()
			{
				Me.AITimer++;
				Me.NPC.velocity *= 0.98f;
			}

			private void DoDiving()
			{
				Me.AITimer++;
				Vector2 direction = Me.Target.Center + Me.Target.velocity * 5f - Me.NPC.Center;
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
				Me.NPC.velocity = (Me.NPC.velocity * (accel - 1) + direction * magnitude) / accel;
			}

			private void DoDisappearing()
			{
				Me.AITimer++;
				Me.NPC.velocity.X *= 0.97f;
				Me.NPC.velocity.Y = Math.Min(Me.NPC.velocity.Y + 0.3f, 18f);
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
							if (!Me.Target.dead && (Me.AITimer > disappearTimerMax || Me.NPC.position.Y > Me.Target.BottomLeft.Y + 800))
							{
								// Go 800 coordinates below the player
								Reset();
								MoveNext(MWCommand.Emerge);
								if (Main.netMode != NetmodeID.MultiplayerClient)
								{
									Me.NPC.netUpdate = true;
								}
							}
							break;
						case MWState.Emerging:
							if (Me.NPC.Top.Y < Me.Location.Y)
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
							if (Me.AITimer > 24 || Me.NPC.velocity.LengthSquared() <= 14f * 14f)
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
							else if (Me.NPC.Bottom.Y > Me.Target.Top.Y)
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
			if (NPC.velocity.X < 0f)
			{
				NPC.spriteDirection = 1;
			}
			else if (NPC.velocity.X > 0f)
			{
				NPC.spriteDirection = -1;
			}

			FSM.UpdateState();
			FSM.ExecuteCurrentState();
			//Main.NewText(FSM.CurrentState);
			//Main.NewText(npc.velocity.Length());
			SpawnGroundFireAndDoScreenShake();

			NPC.rotation = NPC.velocity.ToRotation() + 1.57f;

			if (Main.netMode == NetmodeID.Server && !Synced)
			{
				NPC.netUpdate = true;
				Synced = true;
			}
		}

		private bool TileAirOrNonSolid(Tile tile)
		{
			// If air, or if non-actuated and not a solid
			return !tile.HasUnactuatedTile || (tile.HasTile && !Main.tileSolid[tile.TileType]);
		}

		private bool TileSolid(Tile tile)
		{
			// If non-actuated and a solid or solid top
			return tile.HasUnactuatedTile && (Main.tileSolid[tile.TileType] || Main.tileSolidTop[tile.TileType]);
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
					int damage = (int)(NPC.damage / (Main.GameModeInfo.EnemyDamageMultiplier * 2 * 2));

					Projectile.NewProjectile(NPC.GetSource_FromThis(), position.X, position.Y, 0, 1, type, damage, 0, Main.myPlayer);
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
			float velocitySQ = NPC.velocity.LengthSquared();

			if (velocitySQ < 5f * 5f) return; // Prevent fire spawn if moving slowly (only happens when manually spawned via cheatsheet)

			int type = ModContent.ProjectileType<FireProjHostile>();

			Point16 bottomCenter = NPC.Bottom.ToTileCoordinates16();

			SpawnFires(bottomCenter, type, actuallySpawnFires);
			if (SpawnedFires.Count <= 0 && velocitySQ > 16 * 16)
			{
				// If the NPC moves so fast that it starts skipping tiles, try spawning fires again at a different place
				// This pretty much guarantees fires
				Point16 center = NPC.Center.ToTileCoordinates16();
				SpawnFires(center, type, actuallySpawnFires);
			}

			if (SpawnedFires.Count > 0 && Main.netMode != NetmodeID.Server)
			{
				DoScreenShake(NPC.Center);
			}

			SpawnedFires.Clear();
		}

		public void SpawnFireBalls()
		{
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				int amount = Math.Min((int)(2f * NPC.lifeMax / NPC.life), 10);

				if (Main.expertMode)
				{
					amount += 2;
				}

				float degrees = 5f;
				Vector2 direction = -Vector2.UnitY;

				float distanceX = Target.Center.X + Target.velocity.X - NPC.Center.X;
				int sign = (distanceX > 0).ToDirectionInt();
				float tilt = 20 * Math.Min(1f, Math.Abs(distanceX) / 600);

				direction = direction.RotatedBy(MathHelper.ToRadians(sign * tilt));
				direction = direction.RotatedBy(-MathHelper.ToRadians(-degrees / 2 + degrees * amount / 2));
				int damage = (int)(NPC.damage / (Main.GameModeInfo.EnemyDamageMultiplier * 2 * 4));
				for (int i = 0; i < amount; i++)
				{
					Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Top, direction * 10f, ModContent.ProjectileType<FireballGravityBouncy>(), damage, 0f, Main.myPlayer);
					direction = direction.RotatedBy(MathHelper.ToRadians(degrees));
				}
			}
		}

		private void BodyAI()
		{
			Vector2 pCenter = Parent.Center;
			float parentRotation = Parent.rotation;

			NPC.velocity = Vector2.Zero;
			Vector2 newVelocity = pCenter - NPC.Center;
			if (parentRotation != NPC.rotation)
			{
				float rotatedBy = MathHelper.WrapAngle(parentRotation - NPC.rotation);
				newVelocity = newVelocity.RotatedBy(rotatedBy * 0.1f);
			}

			if (IsHeadExtension)
			{
				// The extension points the other way
				newVelocity = -Parent.velocity;
			}

			NPC.rotation = newVelocity.ToRotation() + 1.57f;

			// Rearrange position based on scale
			NPC.position = NPC.Center;
			NPC.width = NPC.height = (int)(defaultSize * NPC.scale);
			NPC.Center = NPC.position;

			if (newVelocity != Vector2.Zero)
			{
				NPC.Center = pCenter - Vector2.Normalize(newVelocity) * positionOffset * NPC.scale;
			}
			NPC.spriteDirection = (newVelocity.X > 0f) ? -1 : 1;
		}

		/// <summary>
		/// Runs the body or head AI
		/// </summary>
		private void AllAI()
		{
			if (!IsHead && ParentWhoAmI >= 0f && ParentWhoAmI < Main.maxNPCs)
			{
				if (Parent.ModNPC is MagmaWorm mwb && mwb != null)
				{
					BodyAI();
				}
				else
				{
					NPC.active = false; // Gets handled in HandleSegments
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

		public override void HitEffect(NPC.HitInfo hit)
		{
			if (NPC.life > 0)
			{
				int i = 0;
				while (i < hit.Damage / NPC.lifeMax * 50.0)
				{
					Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Torch, 0f, 0f, 0, default(Color), 1.5f);
					dust.velocity *= 1.5f;
					dust.noGravity = true;
					i++;
				}
			}
			else
			{
				for (int i = 0; i < 10; i++)
				{
					Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Torch, 0f, 0f, 0, default(Color), 1.5f);
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

			int count = Main.npcFrameCount[NPC.type];

			if (frameOffset == -1)
			{
				int offset = NPC.whoAmI % count;
				frameOffset = offset;
				frame = offset;
			}

			NPC.frameCounter++;
			if (NPC.frameCounter > 8)
			{
				NPC.frameCounter = 0;
				frame++;
				if (frame >= count + frameOffset)
				{
					frame = frameOffset;
				}
			}

			NPC.frame.Y = (frame % count) * frameHeight;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
		{
			if (!IsHead) return false;

			for (int i = 0; i < Main.maxNPCs; i++)
			{
				NPC n = Main.npc[i];

				if (n.active && n.realLife == NPC.whoAmI && n.ModNPC is not MagmaWormHeadExtension)
				{
					float yoff = 56f * n.scale + 4f; // No clue why those magic numbers, 56 seems to be the distance between top pixel and first "body" pixel
					Texture2D texture = TextureAssets.Npc[n.type].Value;
					Rectangle frame = n.frame;
					Vector2 drawOrigin = frame.Size() / 2;

					Vector2 drawPos = new Vector2(n.Center.X - Main.screenPosition.X - frame.Width * n.scale / 2f + drawOrigin.X * n.scale, n.position.Y - Main.screenPosition.Y + n.height - frame.Height * n.scale + drawOrigin.Y * n.scale + yoff);
					SpriteEffects spriteEffects = n.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
					spriteBatch.Draw(texture, drawPos, frame, n.GetAlpha(drawColor), n.rotation, drawOrigin, n.scale, spriteEffects, 0f);
				}
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

			for (int i = 0; i < Main.maxNPCs; i++)
			{
				NPC tailNPC = Main.npc[i];

				if (tailNPC.active && tailNPC.ModNPC is MagmaWormTail && tailNPC.realLife == NPC.whoAmI)
				{
					// Calculate position between head and tail
					newPos += tailNPC.position;
					newPos /= 2f;

					position = newPos;
					scale = NPC.scale;
					break;
				}
			}
			return true;
		}

		public override void ModifyNPCLoot(NPCLoot npcLoot)
		{
			//TODO redo this so it doesnt spawn items in terrain, but like destroyer does
			npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NeverTrue(), ModContent.ItemType<BurningWitness>()));
			npcLoot.Add(ItemDropRule.Common(ItemID.Obsidian, 1, 20, 25));
			npcLoot.Add(ItemDropRule.Common(ItemID.Hellstone, 1, 10, 13));
		}

		public override void OnKill()
		{
			//TODO redo this so it doesnt spawn items in terrain, but like destroyer does

			//NPCLoot is only called on the head anyway: if (npc.realLife >= 0 && npc.realLife != npc.whoAmI)
			RORGlobalNPC.DropItemInstanced(NPC, NPC.position, NPC.Hitbox.Size(), ModContent.ItemType<BurningWitness>(),
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

			NPC.SetEventFlagCleared(ref RORWorld.downedMagmaWorm, -1);
		}

		public override bool CheckDead()
		{
			if (!SpawnedDead)
			{
				SpawnedDead = true;
				NPC.damage = 0;
				NPC.life = NPC.lifeMax;
				NPC.dontTakeDamage = true;
				//npc.netUpdate = true;
				NPC.NPCLoot();
				SoundEngine.PlaySound(SoundID.Roar.WithPitchOffset(-0.18f), NPC.position);
				SpawnDead();

				return false;
			}
			return true;
		}

		private void SpawnDead()
		{
			int newHead = ModContent.NPCType<MagmaWormDeadHead>();
			int newBody = ModContent.NPCType<MagmaWormDeadBody>();
			int newTail = ModContent.NPCType<MagmaWormDeadTail>();
			for (int i = 0; i < Main.maxNPCs; i++)
			{
				NPC n = Main.npc[i];

				if (n.active && n.realLife == NPC.whoAmI && n.ModNPC is not MagmaWormHeadExtension)
				{
					if (n.ModNPC is not MagmaWorm worm) continue;

					int newType = worm.IsHead ? newHead : (worm.IsTail ? newTail : newBody);
					NPCHelper.Transform(n, newType, worm.Scale, true, true);
				}
			}
		}

		/// <summary>
		/// Calculates the shakeTimer intensity based on distance to local player, and plays a sound
		/// </summary>
		public static void DoScreenShake(Vector2 position)
		{
			SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode.WithVolumeScale(0.5f).WithPitchOffset(Main.rand.NextFloat(-0.6f, -0.4f)), position);
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
