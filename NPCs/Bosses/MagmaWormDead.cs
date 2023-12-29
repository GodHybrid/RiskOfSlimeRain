using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;

namespace RiskOfSlimeRain.NPCs.Bosses
{
	public abstract class MagmaWormDead : MagmaWorm
	{
		public override bool IsHead => this is MagmaWormDeadHead;

		public override bool IsTail => this is MagmaWormDeadTail;

		public override bool IsOtherBody(NPC other) => other.ModNPC is MagmaWormDeadBody || other.ModNPC is MagmaWormDeadTail;

		public int DisappearTimer = 0;

		public override void SetDefaults()
		{
			base.SetDefaults();
			NPC.value = 0;
			NPC.boss = false;
		}

		public override void HeadAI()
		{
			DisappearTimer++;
			if (DisappearTimer > 240)
			{
				NPC.active = false;
				return;
			}

			if (NPC.velocity.X < 0f)
			{
				NPC.spriteDirection = 1;
			}
			else if (NPC.velocity.X > 0f)
			{
				NPC.spriteDirection = -1;
			}

			SpawnGroundFireAndDoScreenShake(false);

			NPC.velocity.X *= 0.986f;
			NPC.velocity.Y = Math.Min(NPC.velocity.Y + 0.3f, 18f);

			NPC.rotation = NPC.velocity.ToRotation() + 1.57f;

			if (Main.netMode == NetmodeID.Server && !Synced)
			{
				NPC.netUpdate = true;
				Synced = true;
			}
		}

		public override bool? CanBeHitByItem(Player player, Item item)
		{
			return false;
		}

		public override bool? CanBeHitByProjectile(Projectile projectile)
		{
			return false;
		}

		public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
		{
			return false;
		}

		public override void OnKill()
		{

		}

		public override bool CheckDead()
		{
			return true;
		}
	}

	public class MagmaWormDeadHead : MagmaWormDead
	{

	}

	public class MagmaWormDeadBody : MagmaWormDead
	{

	}

	public class MagmaWormDeadTail : MagmaWormDead
	{

	}
}
