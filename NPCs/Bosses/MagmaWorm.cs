using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WebmilioCommons.Tinq;

namespace RiskOfSlimeRain.NPCs.Bosses
{
	//TODO MagmaWorm
	// Animation
	// Energized version
	// Death Animation
	// AI
	// Telegraphing (Visuals and Sound)
	// Summon item, boss item
	public abstract class MagmaWorm : ModNPC
	{
		public int head = -1;
		public int body = -1;
		public int tail = -1;
		public const int defaultSize = 44;
		public const int positionOffset = 42;

		public virtual bool IsHead => false;

		public virtual bool IsTail => false;

		public bool IsOtherBody(int other) => other == body || other == tail;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Magma Worm");
		}

		public sealed override void SetDefaults()
		{
			npc.noTileCollide = true;
			npc.Size = new Vector2(defaultSize);
			npc.aiStyle = -1; //6
			npc.netAlways = true;
			npc.damage = 80;
			npc.defense = 10;
			npc.lifeMax = 4000;
			npc.HitSound = SoundID.NPCHit7;
			npc.DeathSound = SoundID.NPCDeath8;
			npc.noGravity = true;
			npc.behindTiles = true;
			npc.knockBackResist = 0f;
			npc.value = 10000f;
			npc.scale = 1.5f;
			npc.buffImmune[BuffID.Poisoned] = true;
			npc.buffImmune[BuffID.OnFire] = true;
			npc.buffImmune[BuffID.CursedInferno] = true;

			head = ModContent.NPCType<MagmaWormHead>();
			body = ModContent.NPCType<MagmaWormBody>();
			tail = ModContent.NPCType<MagmaWormTail>();

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

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(Scale);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			Scale = reader.ReadSingle();
		}

		public int ChildWhoAmI
		{
			get => (int)npc.ai[0];
			private set => npc.ai[0] = value;
		}

		public int ParentWhoAmI
		{
			get => (int)npc.ai[1];
			private set => npc.ai[1] = value;
		}

		public int AttachedHealthWhoAmI
		{
			get => (int)npc.ai[3];
			private set => npc.ai[3] = value;
		}

		public bool Synced
		{
			get => npc.localAI[0] != 1f;
			private set => npc.localAI[0] = value ? 1f : 0f;
		}

		public float Scale
		{
			get => npc.localAI[1];
			private set => npc.localAI[1] = value;
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
					int maxSegments = 16;
					float nextScaleStep = (npc.scale / maxSegments) * 0.60f; // Last segment will have 40% the starting scale
					float nextScale = npc.scale - nextScaleStep;
					for (int k = 0; k < maxSegments; k++)
					{
						int selectedType = body;
						if (k == maxSegments - 1)
						{
							selectedType = tail;
						}

						int childWhoAmI = NPC.NewNPC((int)npc.Center.X, (int)npc.Bottom.Y, selectedType, npc.whoAmI);
						NPC childNPC = Main.npc[childWhoAmI];

						childNPC.realLife = npc.whoAmI;
						MagmaWorm childMWB = childNPC.modNPC as MagmaWorm;
						childMWB.AttachedHealthWhoAmI = npc.whoAmI;
						childMWB.ParentWhoAmI = parentWhoAmI;
						childMWB.Scale = nextScale;

						NPC parentNPC = Main.npc[parentWhoAmI];
						MagmaWorm parentMWB = parentNPC.modNPC as MagmaWorm;
						parentMWB.ChildWhoAmI = childWhoAmI;

						NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, childWhoAmI, 0f, 0f, 0f, 0, 0, 0);
						parentWhoAmI = childWhoAmI;
						nextScale -= nextScaleStep;
					}
				}

				if (!IsHead)
				{
					if (!Parent.active || !(IsOtherBody(Parent.type) || Parent.type == head))
					{
						npc.life = 0;
						npc.HitEffect(0, 10.0);
						npc.active = false;
						NetMessage.SendData(MessageID.StrikeNPC, -1, -1, null, npc.whoAmI, -1f, 0f, 0f, 0, 0, 0);
					}
				}

				if (!IsTail)
				{
					if (!Child.active || !(IsOtherBody(Child.type) || Child.type == tail))
					{
						npc.life = 0;
						npc.HitEffect(0, 10.0);
						npc.active = false;
						NetMessage.SendData(MessageID.StrikeNPC, -1, -1, null, npc.whoAmI, -1f, 0f, 0f, 0, 0, 0);
					}
				}

				if (!npc.active && Main.netMode == NetmodeID.Server)
				{
					NetMessage.SendData(MessageID.StrikeNPC, -1, -1, null, npc.whoAmI, -1f, 0f, 0f, 0, 0, 0);
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
					magnitude = Math.Min(magnitude + 4 * Me.AITimer / (float)emergingTimerCurveMax, 32);
				}

				Me.EmergeDirection = direction * magnitude;

				// For that nice initial curving
				float accel = Utils.Clamp(timerInvert, 20, emergingTimerCurveMax);
				Me.npc.velocity = (Me.npc.velocity * (accel - 1) + direction * magnitude) / accel;

				// Warning
				if (!Me.EmergeWarning && Me.npc.oldVelocity.Y >= 0 && Me.npc.velocity.Y < Me.npc.oldVelocity.Y)
				{
					Me.EmergeWarning = true;
					Warning();
				}
			}

			private void DoSlowingDown()
			{
				Me.AITimer++;
				//Vector2 direction = Me.Target.Center - Me.npc.Center;
				//direction.Normalize();
				//Me.npc.velocity.X = (Me.npc.velocity.X * (30 - 1) + direction.X * 8) / 30;
				////Me.npc.velocity *= 0.95f;
				//Me.npc.velocity.Y += 0.3f;

				Me.npc.velocity *= 0.98f;
			}

			private void DoDiving()
			{
				//Me.AITimer++;
				//Vector2 direction = Me.Target.Center + Me.Target.velocity * 5f - Me.npc.Center;
				//direction.Normalize();
				//Me.npc.velocity.X = (Me.npc.velocity.X * (2 - 1) + direction.X * 10) / 2;
				//Me.npc.velocity.Y = Math.Min(Me.npc.velocity.Y + 0.3f, 16);

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
				direction *= 1f - (rot / (float)divingTimerCurveMax);

				if (Me.AITimer > divingTimerCurveMax)
				{
					//magnitude = Math.Min(magnitude + 3 * Me.AITimer / (float)divingTimerCurveMax, 24);
				}

				Me.EmergeDirection = direction * magnitude;

				// For that nice initial curving
				float accel = Math.Max(divingTimerCurveMax - Me.AITimer / 2, 14);
				Main.NewText("acc: " + accel);
				Me.npc.velocity = (Me.npc.velocity * (accel - 1) + direction * magnitude) / accel;
			}

			private void DoDisappearing()
			{
				Me.AITimer++;
				Me.npc.velocity.X *= 0.97f;
				Me.npc.velocity.Y = Math.Min(Me.npc.velocity.Y + 0.3f, 16);
				//GeneralHoming();
			}

			private void GeneralHoming()
			{
				Vector2 direction = Me.Target.Center + Me.Target.velocity * 5f - Me.npc.Center;
				if (direction.LengthSquared() > 100 * 100)
				{
					direction.Normalize();
					Homing(direction, 5f, 40f);
				}
			}

			private void Homing(Vector2 direction, float magnitude, float accel)
			{
				/*
				 * What happens here is: Initially, the projectile flies in the opposite direction (6 * 30 = 180 degrees)
				 * And then curves towards the destination for 30 ticks
				 */
				//HomingTimer++;
				//Vector2 direction = target.Center + target.velocity * 5f - projectile.Center;
				//int rot = Utils.Clamp(30 - HomingTimer, 0, 30);
				//direction.Normalize();
				//direction = direction.RotatedBy(MathHelper.ToRadians(6 * rot * Math.Sign(-direction.X)));
				//direction *= MaxHomingSpeed * (1f - (rot / 30f));
				////For that nice initial curving
				//float accel = Utils.Clamp(MaxHomingAccel - HomingTimer, MinHomingAccel, MaxHomingAccel);
				Me.npc.velocity = (Me.npc.velocity * (accel - 1) + direction * magnitude) / accel;
			}

			public override void UpdateState()
			{
				if (Me.WatchdogTimer > watchdogTimerMax)
				{
					Reset();
					MoveNext(MWCommand.Reset); // Reset has a transition from every state
				}
				else
				{
					switch (CurrentState)
					{
						case MWState.Disappearing:
							if (Me.AITimer > disappearTimerMax || Me.npc.position.Y > Me.Target.BottomLeft.Y + 800)
							{
								// Go 2000 coordinates below the player
								Reset();
								MoveNext(MWCommand.Emerge);
							}
							break;
						case MWState.Emerging:
							if (Me.npc.Top.Y < Me.Location.Y)
							{
								Reset();
								//Main.NewText(Me.npc.velocity);
								MoveNext(MWCommand.Slowdown);
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

		private void HeadMovement(Vector2 destination)
		{
			//npc.oldVelocity = npc.velocity; // Because noTileCollide doesn't do it by itself
			if (npc.velocity.X < 0f)
			{
				npc.spriteDirection = 1;
			}
			else if (npc.velocity.X > 0f)
			{
				npc.spriteDirection = -1;
			}

			if (false)
			{
				WyvernHeadMovement(destination);

				//Main.NewText("#####");
				//if (FSM.CurrentState == MWState.Initializing) FSM.MoveNext(MWCommand.Reset);
				//FSM.MoveNext(MWCommand.Emerge);
				//FSM.MoveNext(MWCommand.Slowdown);
				//FSM.MoveNext(MWCommand.Dive);
			}
			else
			{
				FSM.UpdateState();
				FSM.ExecuteCurrentState();
				Main.NewText(FSM.CurrentState);
				//Vector2 center = npc.Center;
				//Vector2 targetCenter = Target.Center;
				////center.X = (int)(center.X / 16f) * 16;
				////center.Y = (int)(center.Y / 16f) * 16;
				//destination = targetCenter - center;

				//Check state machine conditions, including state transitions
				//FSM.UpdateState();
				//FSM.ExecuteCurrentState();
			}

			npc.rotation = npc.velocity.ToRotation() + 1.57f;

			if (!Synced)
			{
				npc.netUpdate = true;
				Synced = true;
			}
		}

		private void WyvernHeadMovement(Vector2 destination)
		{
			float speed = 11f;
			float accel = 0.25f;
			float length = destination.Length();
			float absX = Math.Abs(destination.X);
			float absY = Math.Abs(destination.Y);
			destination *= speed / length;
			bool someMovementBool = false;
			if (((npc.velocity.X > 0f && destination.X < 0f) || (npc.velocity.X < 0f && destination.X > 0f) || (npc.velocity.Y > 0f && destination.Y < 0f) || (npc.velocity.Y < 0f && destination.Y > 0f)) && Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) > accel / 2f && length < 300f)
			{
				someMovementBool = true;
				if (Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) < speed)
				{
					npc.velocity *= 1.1f;
				}
			}
			if (npc.position.Y > Target.position.Y || Target.position.Y / 16f > Main.worldSurface || Target.dead)
			{
				someMovementBool = true;
				if (Math.Abs(npc.velocity.X) < speed / 2f)
				{
					if (npc.velocity.X == 0f)
					{
						npc.velocity.X = npc.velocity.X - npc.direction;
					}
					npc.velocity.X = npc.velocity.X * 1.1f;
				}
				else if (npc.velocity.Y > -speed)
				{
					npc.velocity.Y = npc.velocity.Y - accel;
				}
			}

			if (!someMovementBool)
			{
				if ((npc.velocity.X > 0f && destination.X > 0f) || (npc.velocity.X < 0f && destination.X < 0f) || (npc.velocity.Y > 0f && destination.Y > 0f) || (npc.velocity.Y < 0f && destination.Y < 0f))
				{
					if (npc.velocity.X < destination.X)
					{
						npc.velocity.X = npc.velocity.X + accel;
					}
					else if (npc.velocity.X > destination.X)
					{
						npc.velocity.X = npc.velocity.X - accel;
					}
					if (npc.velocity.Y < destination.Y)
					{
						npc.velocity.Y = npc.velocity.Y + accel;
					}
					else if (npc.velocity.Y > destination.Y)
					{
						npc.velocity.Y = npc.velocity.Y - accel;
					}
					if (Math.Abs(destination.Y) < speed * 0.2f && ((npc.velocity.X > 0f && destination.X < 0f) || (npc.velocity.X < 0f && destination.X > 0f)))
					{
						if (npc.velocity.Y > 0f)
						{
							npc.velocity.Y = npc.velocity.Y + accel * 2f;
						}
						else
						{
							npc.velocity.Y = npc.velocity.Y - accel * 2f;
						}
					}
					if (Math.Abs(destination.X) < speed * 0.2f && ((npc.velocity.Y > 0f && destination.Y < 0f) || (npc.velocity.Y < 0f && destination.Y > 0f)))
					{
						if (npc.velocity.X > 0f)
						{
							npc.velocity.X = npc.velocity.X + accel * 2f;
						}
						else
						{
							npc.velocity.X = npc.velocity.X - accel * 2f;
						}
					}
				}
				else if (absX > absY)
				{
					if (npc.velocity.X < destination.X)
					{
						npc.velocity.X = npc.velocity.X + accel * 1.1f;
					}
					else if (npc.velocity.X > destination.X)
					{
						npc.velocity.X = npc.velocity.X - accel * 1.1f;
					}
					if (Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) < speed * 0.5f)
					{
						if (npc.velocity.Y > 0f)
						{
							npc.velocity.Y = npc.velocity.Y + accel;
						}
						else
						{
							npc.velocity.Y = npc.velocity.Y - accel;
						}
					}
				}
				else
				{
					if (npc.velocity.Y < destination.Y)
					{
						npc.velocity.Y = npc.velocity.Y + accel * 1.1f;
					}
					else if (npc.velocity.Y > destination.Y)
					{
						npc.velocity.Y = npc.velocity.Y - accel * 1.1f;
					}
					if (Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) < speed * 0.5f)
					{
						if (npc.velocity.X > 0f)
						{
							npc.velocity.X = npc.velocity.X + accel;
						}
						else
						{
							npc.velocity.X = npc.velocity.X - accel;
						}
					}
				}
			}
		}

		private void BodyMovement()
		{
			Vector2 pCenter = Parent.Center;
			float parentRotation = Parent.rotation;
			float scaleOffset = MathHelper.Clamp(npc.scale, 0f, 50f);

			npc.velocity = Vector2.Zero;
			Vector2 newVelocity = pCenter - npc.Center;
			if (parentRotation != npc.rotation)
			{
				float rotatedBy = MathHelper.WrapAngle(parentRotation - npc.rotation);
				newVelocity = newVelocity.RotatedBy(rotatedBy * 0.1f);
			}

			npc.rotation = newVelocity.ToRotation() + 1.57f;

			// Rearrange position based on scale
			npc.position = npc.Center;
			npc.scale = scaleOffset;
			npc.width = npc.height = (int)(defaultSize * npc.scale);
			npc.Center = npc.position;

			if (newVelocity != Vector2.Zero)
			{
				npc.Center = pCenter - Vector2.Normalize(newVelocity) * positionOffset * scaleOffset;
			}
			npc.spriteDirection = (newVelocity.X > 0f) ? -1 : 1;
		}

		private void Movement()
		{
			Vector2 center = npc.Center;
			Vector2 targetCenter = Target.Center;
			center.X = (int)(center.X / 16f) * 16;
			center.Y = (int)(center.Y / 16f) * 16;
			Vector2 destination = targetCenter - center;
			if (!IsHead && ParentWhoAmI >= 0f && ParentWhoAmI <= Main.maxNPCs)
			{
				if (Parent.modNPC is MagmaWorm mwb && mwb != null)
				{
					BodyMovement();
				}
				else
				{
					npc.active = false; // Gets handled in HandleSegments
				}
			}
			else if (IsHead)
			{
				HeadMovement(destination);
			}
		}

		public sealed override void AI()
		{
			TargetingAndEssentials();

			HandleSegments();

			Movement();
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

		public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
		{
			float yoff = 56f * npc.scale + 4f; // No clue why those magic numbers
			Texture2D texture = Main.npcTexture[npc.type];
			Vector2 drawOrigin = new Vector2(texture.Width >> 1, (texture.Height >> 1) / Main.npcFrameCount[npc.type]);

			Vector2 drawPos = new Vector2(npc.Center.X - Main.screenPosition.X - texture.Width * npc.scale / 2f + drawOrigin.X * npc.scale, npc.position.Y - Main.screenPosition.Y + npc.height - texture.Height * npc.scale / Main.npcFrameCount[npc.type] + drawOrigin.Y * npc.scale + yoff);
			SpriteEffects spriteEffects = npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			spriteBatch.Draw(texture, drawPos, npc.frame, Color.White, npc.rotation, drawOrigin, npc.scale, spriteEffects, 0f);

			return false;
		}

		public override void PostDraw(SpriteBatch spriteBatch, Color drawColor)
		{
			//TODO move this somewhere else because its blocked by tiles
			if (!IsHead) return;

			Utils.DrawLine(Main.spriteBatch, Target.Center, Target.Center + npc.velocity * 10f, Color.White, Color.White, 2);
			Utils.DrawLine(Main.spriteBatch, Target.Center, Target.Center + EmergeDirection * 10f, Color.Green, Color.Green, 2);

			if (FSM.CurrentState != MWState.Emerging) return;
			if (!EmergeWarning) return;
			if (Location == Vector2.Zero) return;
			Vector2 drawCenter = Location - Main.screenPosition;
			Texture2D texture = ModContent.GetTexture("RiskOfSlimeRain/Textures/Slowdown");
			Rectangle destination = Utils.CenteredRectangle(drawCenter, texture.Size());
			destination.Inflate(10, 10);
			spriteBatch.Draw(texture, destination, Color.White);
		}

		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			if (!IsHead) return false;

			Vector2 newPos = position;

			NPC tailNPC = Main.npc.FirstActiveOrDefault(n => n.type == tail && n.realLife == npc.realLife);
			if (tailNPC != null)
			{
				newPos += tailNPC.position;
				newPos /= 2f;
				position = newPos;
				scale = npc.scale;
			}
			return true;
		}

		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
		{
			return IsHead;
		}
	}

	public class MagmaWormHead : MagmaWorm
	{
		public override bool IsHead => true;
	}

	public class MagmaWormBody : MagmaWorm
	{

	}

	public class MagmaWormTail : MagmaWorm
	{
		public override bool IsTail => true;
	}
}
