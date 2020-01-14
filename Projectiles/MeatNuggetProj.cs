using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WebmilioCommons.Tinq;

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
			projectile.frame = TextureIndex;
			projectile.rotation = 0;
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

		//1 2 3 4
		public int TextureIndex => (projectile.identity % (Main.projFrames[projectile.type] - 1)) + 1;

		public override void AI()
		{
			if (projectile.frame == 0) projectile.rotation = projectile.velocity.ToRotation();
			else projectile.rotation = 0;

			projectile.velocity.Y += 0.2f;
			if (projectile.velocity.Y > 13f)
			{
				projectile.velocity.Y = 13f;
			}

			if (projectile.owner == Main.myPlayer)
			{
				if (Main.netMode == NetmodeID.SinglePlayer)
				{
					if (Main.LocalPlayer.Hitbox.Intersects(projectile.Hitbox))
					{
						Main.PlaySound(SoundID.Item86.SoundId, (int)projectile.Center.X, (int)projectile.Center.Y, SoundID.Item86.Style, 0.7f, 0.6f);
						Main.LocalPlayer.HealMe(Heal);
						projectile.Kill();
					}
				}
				else
				{
					bool healed = false;
					Main.player.WhereActive(p => p.Hitbox.Intersects(projectile.Hitbox)).Do(delegate(Player p)
					{
						if (!healed)
						{
							healed = true;
							SoundHelper.PlaySound(SoundID.Item86.SoundId, (int)projectile.Center.X, (int)projectile.Center.Y, SoundID.Item86.Style, 0.7f, 0.6f);
							p.HealMe(Heal);
							projectile.Kill();
						}
					});
				}
			}
		}
	}
}
