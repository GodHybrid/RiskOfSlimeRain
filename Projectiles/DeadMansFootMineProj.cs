﻿using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WebmilioCommons.Extensions;
using WebmilioCommons.Tinq;

namespace RiskOfSlimeRain.Projectiles
{
	/// <summary>
	/// ai0 - player damage, ai1 - ticks
	/// </summary>
	public class DeadMansFootMineProj : ModProjectile, IExcludeOnHit
	{
		public bool activate = false;
		public byte addRadiusX = 20;
		public byte addRadiusY = 10;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Dead Man's Mine");
			Main.projFrames[projectile.type] = 7;
		}

		public override void SetDefaults()
		{
			projectile.Size = new Vector2(28, 28);
			projectile.timeLeft = 3600;
			projectile.friendly = true;
			projectile.penetrate = -1;
			drawOriginOffsetY = 2;
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			projectile.velocity = Vector2.Zero;
			return false;
		}

		public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
		{
			fallThrough = false;
			return base.TileCollideStyle(ref width, ref height, ref fallThrough);
		}

		public override bool? CanCutTiles()
		{
			return false;
		}

		public int Damage
		{
			get => (int)projectile.ai[0];
			set => projectile.ai[0] = value;
		}

		public int Ticks
		{
			get => (int)projectile.ai[1];
			set => projectile.ai[1] = value;
		}

		public override void AI()
		{
			projectile.velocity.Y += 0.5f;
			projectile.LoopAnimation(7);
			if (Main.myPlayer == projectile.owner)
			{
				bool explode = Main.npc.CountActive(n => n.CanBeChasedBy() && n.Hitbox.Intersects(projectile.Hitbox)) > 0;
				if (explode)
				{
					Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<DeadMansFootExplosionProj>(), 0, 0, Main.myPlayer);
					projectile.Kill();
				}
			}
		}

		public override void Kill(int timeLeft)
		{
			if (Main.myPlayer == projectile.owner && timeLeft > 0)
			{
				Rectangle explosionArea = projectile.Hitbox;
				explosionArea.Inflate(addRadiusX, addRadiusY);
				Main.npc.WhereActive(n => n.CanBeChasedBy() && n.Hitbox.Intersects(explosionArea)).Do(n =>
				{
					n.AddBuff(BuffID.Venom, 30 * Ticks);
					int damage = (int)(1.5f * Damage);
					StickyProj.NewProjectile(n, damage: damage, onCreate: delegate (DeadMansFootDoTProj t)
					{
						t.TimeLeft = (ushort)(30 * Ticks + 20);
					});
				});
			}
			Main.PlaySound(SoundID.DD2_ExplosiveTrapExplode?.WithVolume(0.8f), projectile.Center);
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White;
		}
	}
}
