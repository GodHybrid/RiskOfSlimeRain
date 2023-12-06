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
	/// ai0 damage
	/// </summary>
	public class PanicMinesProj : ModProjectile, IExcludeOnHit
	{
		public bool activate = false;
		public byte timer = 25;
		public byte addRadiusX = 50;
		public byte addRadiusY = 30;

		public int Damage
		{
			get => (int)Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}

		public override void SetStaticDefaults()
		{
			Main.projFrames[Projectile.type] = 4;
		}

		public override void SetDefaults()
		{
			base.SetDefaults();
			Projectile.Size = new Vector2(14, 36);
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

		public override void AI()
		{
			Projectile.velocity.Y += 0.5f;
			Projectile.LoopAnimation(4);

			if (!activate)
			{
				for (int i = 0; i < Main.maxNPCs; i++)
				{
					NPC npc = Main.npc[i];

					if (npc.active && npc.CanBeChasedBy() && npc.Hitbox.Intersects(Projectile.Hitbox))
					{
						activate = true;
					}
				}
			}

			if (activate)
			{
				if (timer >= 0 && timer % 5 == 0)
				{
					SoundEngine.PlaySound(SoundID.DD2_CrystalCartImpact.WithVolumeScale(0.8f).WithPitchOffset(0.6f), Projectile.Center);
				}
				timer--;
			}
			if (timer == 0 && Main.myPlayer == Projectile.owner)
			{
				Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<PanicMinesExplosionProj>(), 0, 0, Main.myPlayer);
				Projectile.Kill();
			}
		}

		public override void OnKill(int timeLeft)
		{
			Rectangle explosionArea = Projectile.Hitbox;
			explosionArea.Inflate(addRadiusX / 2, addRadiusY / 2);
			if (Main.myPlayer == Projectile.owner)
			{
				for (int i = 0; i < Main.maxNPCs; i++)
				{
					NPC n = Main.npc[i];

					if (n.active && n.CanBeChasedBy() && n.Hitbox.Intersects(explosionArea))
					{
						n.SimpleStrikeNPC(Damage, 0, damageType: ModContent.GetInstance<ArmorPenDamageClass>()); //Does not proc, syncs
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
