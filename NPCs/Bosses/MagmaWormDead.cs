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

		public override bool IsOtherBody(NPC other) => other.modNPC is MagmaWormDeadBody || other.modNPC is MagmaWormDeadTail;

		public int DisappearTimer = 0;

		public override void HeadAI()
		{
			DisappearTimer++;
			if (DisappearTimer > 240)
			{
				npc.active = false;
				return;
			}

			if (npc.velocity.X < 0f)
			{
				npc.spriteDirection = 1;
			}
			else if (npc.velocity.X > 0f)
			{
				npc.spriteDirection = -1;
			}

			SpawnGroundFireAndDoScreenShake(false);

			npc.velocity.X *= 0.986f;
			npc.velocity.Y = Math.Min(npc.velocity.Y + 0.3f, 18f);

			npc.rotation = npc.velocity.ToRotation() + 1.57f;

			if (Main.netMode == NetmodeID.Server && !Synced)
			{
				npc.netUpdate = true;
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

		public override void NPCLoot()
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
