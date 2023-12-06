using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

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
			Main.projFrames[Projectile.type] = 7;
		}

		public override void SetDefaults()
		{
			Projectile.Size = new Vector2(28, 28);
			Projectile.timeLeft = 3600;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
			DrawOriginOffsetY = 2;
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			Projectile.velocity = Vector2.Zero;
			return false;
		}

		public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
		{
			fallThrough = false;
			return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
		}

		public override bool? CanCutTiles()
		{
			return false;
		}

		public int Damage
		{
			get => (int)Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}

		public int Ticks
		{
			get => (int)Projectile.ai[1];
			set => Projectile.ai[1] = value;
		}

		public override void AI()
		{
			Projectile.velocity.Y += 0.5f;
			Projectile.LoopAnimation(7);
			if (Main.myPlayer == Projectile.owner)
			{
				for (int i = 0; i < Main.maxNPCs; i++)
				{
					NPC npc = Main.npc[i];

					if (npc.active && npc.CanBeChasedBy() && npc.Hitbox.Intersects(Projectile.Hitbox))
					{
						Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<DeadMansFootExplosionProj>(), 0, 0, Main.myPlayer);
						Projectile.Kill();
						break;
					}
				}
			}
		}

		public override void OnKill(int timeLeft)
		{
			if (Main.myPlayer == Projectile.owner && timeLeft > 0)
			{
				Rectangle explosionArea = Projectile.Hitbox;
				explosionArea.Inflate(addRadiusX, addRadiusY);
				for (int i = 0; i < Main.maxNPCs; i++)
				{
					NPC n = Main.npc[i];

					if (n.active && n.CanBeChasedBy() && n.Hitbox.Intersects(explosionArea))
					{
						n.AddBuff(BuffID.Venom, 30 * Ticks);
						int damage = (int)(1.5f * Damage);
						StickyProj.NewProjectile(Projectile.GetSource_FromThis(), n, damage: damage, onCreate: delegate (DeadMansFootDoTProj t)
						{
							t.TimeLeft = (ushort)(30 * Ticks + 20);
						});
					}
				}
			}
			SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode.WithVolumeScale(0.8f), Projectile.Center);
		}

		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White;
		}
	}
}
