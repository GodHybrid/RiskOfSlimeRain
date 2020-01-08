using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Projectiles
{
	public class MeatNuggetProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Meat Nugget");
			Main.projFrames[projectile.type] = 5;
			ProjectileID.Sets.NeedsUUID[projectile.type] = true;
		}

		public override void SetDefaults()
		{
			projectile.width = 18;
			projectile.height = 12;
			projectile.friendly = true;
			projectile.penetrate = -1;
			//projectile.tileCollide = true;
			projectile.timeLeft = 1800;
			drawOriginOffsetY = 4;
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			projectile.velocity = Vector2.Zero;
			//1 2 3 4
			projectile.frame = (projectile.identity % (Main.projFrames[projectile.type] - 1)) + 1;
			return false;
		}

		public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
		{
			//so it sticks to platforms
			fallThrough = false;
			return base.TileCollideStyle(ref width, ref height, ref fallThrough);
		}

		public int Heal
		{
			get => (int)projectile.ai[0];
			set => projectile.ai[0] = value;
		}

		public override void AI()
		{
			if (projectile.frame == 0) projectile.rotation = projectile.velocity.ToRotation();
			else projectile.rotation = 0;
			projectile.velocity.Y += 0.2f;
			if (projectile.velocity.Y > 13f)
			{
				projectile.velocity.Y = 13f;
			}
			if (projectile.Hitbox.Intersects(Main.LocalPlayer.Hitbox))
			{
				if (projectile.owner == Main.myPlayer)
				{
					projectile.GetOwner().HealMe(Heal);
				}
				Main.PlaySound(SoundID.Item86.SoundId, (int)projectile.Center.X, (int)projectile.Center.Y, SoundID.Item86.Style, 0.7f, 0.6f);
				projectile.Kill();
			}
		}
	}
}
