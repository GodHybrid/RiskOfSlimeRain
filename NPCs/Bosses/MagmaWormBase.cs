using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.NPCs.Bosses
{
	//TODO MagmaWorm
	// Animation
	// Energized version
	// Death Animation
	// AI
	// Telegraphing (Visuals and Sound)
	// Summon item, boss item
	public abstract class MagmaWormBase : ModNPC
	{
		public int head = -1;
		public int body = -1;
		public int tail = -1;
		public const int defaultSize = 32;

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
					int maxSegments = 24;
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
						MagmaWormBase childMWB = childNPC.modNPC as MagmaWormBase;
						childMWB.AttachedHealthWhoAmI = npc.whoAmI;
						childMWB.ParentWhoAmI = parentWhoAmI;
						childMWB.Scale = nextScale;

						NPC parentNPC = Main.npc[parentWhoAmI];
						MagmaWormBase parentMWB = parentNPC.modNPC as MagmaWormBase;
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

		private void HeadMovement(Vector2 destination)
		{
			if (npc.velocity.X < 0f)
			{
				npc.spriteDirection = 1;
			}
			else if (npc.velocity.X > 0f)
			{
				npc.spriteDirection = -1;
			}

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
			npc.rotation = npc.velocity.ToRotation() + 1.57f;

			if (!Synced)
			{
				npc.netUpdate = true;
				Synced = true;
			}
		}

		private void BodyMovement()
		{
			Vector2 pCenter = Parent.Center;
			float parentRotation = Parent.rotation;
			float scaleOffset = MathHelper.Clamp(npc.scale, 0f, 50f);
			float positionOffset = 42f;

			npc.velocity = Vector2.Zero;
			Vector2 newVelocity = pCenter - npc.Center;
			if (parentRotation != npc.rotation)
			{
				float rotatedBy = MathHelper.WrapAngle(parentRotation - npc.rotation);
				newVelocity = newVelocity.RotatedBy(rotatedBy * 0.1f);
			}

			npc.rotation = newVelocity.ToRotation() + 1.57f;

			//Rearrange position based on scale
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
				if (Parent.modNPC is MagmaWormBase mwb && mwb != null)
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
			float yoff = 56f * npc.scale + 4f;
			Texture2D texture = Main.npcTexture[npc.type];
			Vector2 drawOrigin = new Vector2(texture.Width >> 1, (texture.Height >> 1) / Main.npcFrameCount[npc.type]);

			Vector2 drawPos = new Vector2(npc.Center.X - Main.screenPosition.X - texture.Width * npc.scale / 2f + drawOrigin.X * npc.scale, npc.position.Y - Main.screenPosition.Y + npc.height - texture.Height * npc.scale / Main.npcFrameCount[npc.type] + drawOrigin.Y * npc.scale + yoff);
			SpriteEffects spriteEffects = npc.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
			spriteBatch.Draw(texture, drawPos, npc.frame, Color.White, npc.rotation, drawOrigin, npc.scale, spriteEffects, 0f);

			return false;
		}

		public override bool CanHitPlayer(Player target, ref int cooldownSlot)
		{
			return IsHead;
		}
	}

	public class MagmaWormHead : MagmaWormBase
	{
		public override bool IsHead => true;
	}

	public class MagmaWormBody : MagmaWormBase
	{

	}

	public class MagmaWormTail : MagmaWormBase
	{
		public override bool IsTail => true;
	}
}
