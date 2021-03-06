﻿using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.NPCEffects;
using RiskOfSlimeRain.Core.NPCEffects.Common;
using RiskOfSlimeRain.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WebmilioCommons.Extensions;
using WebmilioCommons.Tinq;

namespace RiskOfSlimeRain.Projectiles
{
	/// <summary>
	/// ai0 is used to set timeLeft
	/// </summary>
	public class SpikestripProj : ModProjectile
	{
		public override void SetDefaults()
		{
			projectile.width = 34;
			projectile.height = 4;
			projectile.friendly = true;
			projectile.penetrate = -1;
			//projectile.tileCollide = true;
			projectile.timeLeft = 300;
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			projectile.velocity = Vector2.Zero;
			return false;
		}

		public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
		{
			//so it sticks to platforms
			fallThrough = false;
			return base.TileCollideStyle(ref width, ref height, ref fallThrough);
		}

		public override bool? CanCutTiles()
		{
			return false;
		}

		public int Duration
		{
			get => (int)projectile.ai[0];
			set => projectile.ai[0] = value;
		}

		public override void AI()
		{
			if (projectile.localAI[0] == 0f)
			{
				projectile.localAI[0] = 1f;
				projectile.timeLeft = Duration;
			}

			projectile.velocity.Y += 0.5f;
			Main.npc.WhereActive(n => n.CanBeChasedBy() && !n.boss && n.Hitbox.Intersects(projectile.Hitbox)).Do(delegate (NPC n)
			{
				if (NPCHelper.IsBossPiece(n)) return;
				if (n.type == NPCID.WallofFlesh || n.type == NPCID.WallofFleshEye) return;
				if (NPCHelper.IsWormBodyOrTail(n)) return;
				NPCEffectManager.ApplyNPCEffect<SpikestripNPCEffect>(n, 60);
			});
		}
	}
}
