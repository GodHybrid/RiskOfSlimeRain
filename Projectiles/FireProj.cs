using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Projectiles
{
	public class FireProj : ModProjectile, IExcludeOnHit
	{
		public Rectangle newHitbox = default(Rectangle);

		public int Timer { get; set; }

		private bool timeLeftSet = false;

		public const int timeLeftDefault = 300;

		public int TimeLeft
		{
			get => (int)Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}

		public int SpawnLight => (int)Projectile.ai[1];

		public virtual bool IgnoreTimeLeft => false;

		public override string Texture => "RiskOfSlimeRain/Empty";

		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Fire");
		}

		public override void SetDefaults()
		{
			Projectile.width = 8;
			Projectile.height = 8;
			Projectile.penetrate = -1;
			Projectile.friendly = true;
			Projectile.timeLeft = timeLeftDefault; //Set by TimeLeft through ai[0]
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.noEnchantments = true;

			Projectile.usesIDStaticNPCImmunity = true;
			Projectile.idStaticNPCHitCooldown = 30;
		}

		public override void ModifyDamageHitbox(ref Rectangle hitbox)
		{
			hitbox = newHitbox;
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			Projectile.velocity.Y = 0f;
			return false;
		}

		public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
		{
			//so it sticks to platforms
			fallThrough = false;
			return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
		{
			target.AddBuff(BuffID.OnFire, 60);
		}

		public override void AI()
		{
			SetTimeLeft();
			Hitbox();
			Movement();
			Visuals();
		}

		private void SetTimeLeft()
		{
			if (IgnoreTimeLeft) return;
			if (!timeLeftSet)
			{
				Projectile.timeLeft = TimeLeft < 0 ? timeLeftDefault : TimeLeft; //Set to timeLeftDefault if its not set, otherwise set to specified
				timeLeftSet = true;
			}
		}

		private void Hitbox()
		{
			newHitbox = Projectile.Hitbox;
			newHitbox.Inflate(8, 2);
		}

		private void Movement()
		{
			Timer += 1;

			if (Timer > 5)
			{
				Timer = 5;
				//stop spreading
				Projectile.velocity.X *= 0.9f;
				if (Projectile.velocity.X > -0.01f && Projectile.velocity.X < 0.01f)
				{
					Projectile.velocity.X = 0f;
					Projectile.netUpdate = true;
				}
				//fall down
				Projectile.velocity.Y = Projectile.velocity.Y + 0.2f;
			}

			//terminal velocity cap
			if (Projectile.velocity.Y > 16f)
			{
				Projectile.velocity.Y = 16f;
			}

			//die if wet
			if (Projectile.wet)
			{
				Projectile.Kill();
			}
		}

		private void Visuals()
		{
			if (SpawnLight > 0)
			{
				Lighting.AddLight(Projectile.Center, new Vector3(1f, 0.7f, 0.7f));
			}

			if (Main.rand.NextFloat() < 0.6f) return;
			//fire going up
			Dust dust = Dust.NewDustDirect(newHitbox.TopLeft(), newHitbox.Width, newHitbox.Height, DustID.Torch, 0f, 0f, 100);
			dust.position.X -= 2f;
			dust.position.Y += 2f;
			dust.scale += Main.rand.NextFloat(0.5f);
			dust.noGravity = true;
			dust.velocity.Y -= 2f;
			dust.noLight = true;

			//static fire
			if (Main.rand.NextBool(2))
			{
				dust = Dust.NewDustDirect(newHitbox.TopLeft(), newHitbox.Width, newHitbox.Height, DustID.Torch, 0f, 0f, 100);
				dust.position.X -= 2f;
				dust.position.Y += 2f;
				dust.scale += 0.3f + Main.rand.NextFloat(0.5f);
				dust.noGravity = true;
				dust.velocity *= 0.1f;
				dust.noLight = true;
			}
		}
	}
}
