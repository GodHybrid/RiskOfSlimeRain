using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Projectiles
{
	/// <summary>
	/// Sticks to the target specified via the special spawn method. Doesn't deal damage by itself. ai0 and ai1 reserved.
	/// </summary>
	public abstract class StickyProj : ModProjectile
	{
		/// <summary>
		/// Custom NewProjectile to properly spawn a StickyProj. offset is from the position of the target (defaults to center). damage is clientside. onCreate to set additional variables (clientside). Projectile will be synced after onCreate
		/// </summary>
		public static void NewProjectile<T>(NPC target, Vector2 offset = default(Vector2), int damage = 0, Action<T> onCreate = null) where T : StickyProj
		{
			if (typeof(IOncePerNPC).IsAssignableFrom(typeof(T)))
			{
				for (int i = 0; i < Main.maxProjectiles; i++)
				{
					Projectile p = Main.projectile[i];
					if (p.active && p.modProjectile is T t && t.TargetWhoAmI == target.whoAmI)
					{
						return;
					}
				}
			}

			uint packedOffset;
			Point point;
			if (offset == default(Vector2))
			{
				point = (target.Size / 2).ToPoint();
			}
			else
			{
				point = offset.ToPoint();
			}
			packedOffset = GetPackedOffset(point);

			int index = Projectile.NewProjectile(target.Center, Vector2.Zero, ModContent.ProjectileType<T>(), 0, 0, Main.myPlayer, packedOffset, target.whoAmI);
			if (index < Main.maxProjectiles)
			{
				Projectile p = Main.projectile[index];
				T t = p.modProjectile as T;

				t.damage = damage;
				if (onCreate != null)
				{
					onCreate.Invoke(t);
					p.netUpdate = true;
				}
			}
		}

		private static uint GetPackedOffset(Point offset)
		{
			uint x = ((uint)offset.X << 16) & 0xFFFF0000;
			uint y = (uint)offset.Y;
			return x + y;
		}

		private Vector2 GetOffset()
		{
			uint offY = PackedOffset & 0xFFFF;
			uint offX = (PackedOffset >> 16) & 0xFFFF;
			return new Vector2(offX, offY);
		}

		public override void SetDefaults()
		{
			projectile.width = 16;
			projectile.height = 16;
			projectile.aiStyle = -1;
			projectile.friendly = true;
			projectile.melee = true;
			projectile.penetrate = -1;
			projectile.hide = true;
			projectile.timeLeft = 140;
			projectile.ignoreWater = true; //make sure the projectile ignores water
			projectile.tileCollide = false; //make sure the projectile doesn't collide with tiles anymore
		}

		public int damage = 0;

		//packed offset
		public uint PackedOffset
		{
			get => (uint)projectile.ai[0];
			set => projectile.ai[0] = value;
		}

		//index of the current target
		public int TargetWhoAmI
		{
			get => (int)projectile.ai[1];
			set => projectile.ai[1] = value;
		}

		public override void DrawBehind(int index, List<int> drawCacheProjsBehindNPCsAndTiles, List<int> drawCacheProjsBehindNPCs, List<int> drawCacheProjsBehindProjectiles, List<int> drawCacheProjsOverWiresUI)
		{
			//if attached to an NPC, draw behind tiles (and the npc) if that NPC is behind tiles, otherwise just behind the NPC.
			int npcIndex = TargetWhoAmI;
			if (npcIndex >= 0 && npcIndex < 200 && Main.npc[npcIndex].active)
			{
				if (Main.npc[npcIndex].behindTiles)
				{
					drawCacheProjsBehindNPCsAndTiles.Add(index);
				}
				else
				{
					drawCacheProjsBehindProjectiles.Add(index);
				}
			}
		}

		public sealed override void AI()
		{

			int projTargetIndex = TargetWhoAmI;
			if (projTargetIndex < 0 || projTargetIndex >= 200)
			{
				//if the index is past its limits, kill it
				projectile.Kill();
				return;
			}
			NPC npc = Main.npc[projTargetIndex];

			if (npc.active && !npc.dontTakeDamage)
			{
				//if the target is active and can take damage
				//set the projectile's position relative to the target's position + some offset

				projectile.Center = npc.position + GetOffset();
				projectile.gfxOffY = npc.gfxOffY;

				WhileStuck(npc);
			}
			else
			{
				//otherwise, kill the projectile
				projectile.Kill();
				return;
			}

			OtherAI();
		}

		public virtual void WhileStuck(NPC npc)
		{

		}

		public virtual void OtherAI()
		{

		}
	}
}
