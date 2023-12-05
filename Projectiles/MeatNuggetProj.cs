using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Projectiles
{
	public class MeatNuggetProj : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Meat Nugget");
			Main.projFrames[Projectile.type] = 5;
		}

		public override void SetDefaults()
		{
			Projectile.width = 18;
			Projectile.height = 12;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
			//projectile.tileCollide = true;
			Projectile.timeLeft = 1800;
			DrawOriginOffsetY = 4;
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			Projectile.velocity = Vector2.Zero;
			Projectile.frame = TextureIndex;
			Projectile.rotation = 0;
			return false;
		}

		public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
		{
			//so it sticks to platforms
			fallThrough = false;
			return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
		}

		public int Heal
		{
			get => (int)Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}

		//1 2 3 4
		public int TextureIndex => (Projectile.identity % (Main.projFrames[Projectile.type] - 1)) + 1;

		public override void AI()
		{
			if (Projectile.frame == 0) Projectile.rotation = Projectile.velocity.ToRotation();
			else Projectile.rotation = 0;

			Projectile.velocity.Y += 0.2f;
			if (Projectile.velocity.Y > 13f)
			{
				Projectile.velocity.Y = 13f;
			}

			if (Projectile.owner == Main.myPlayer)
			{
				if (Main.netMode == NetmodeID.SinglePlayer)
				{
					if (Main.LocalPlayer.Hitbox.Intersects(Projectile.Hitbox))
					{
						SoundEngine.PlaySound(SoundID.Item86.WithVolumeScale(0.7f).WithPitchOffset(0.6f), Projectile.Center);
						Main.LocalPlayer.HealMe(Heal);
						Projectile.Kill();
					}
				}
				else
				{
					for (int i = 0; i < Main.maxPlayers; i++)
					{
						Player player = Main.player[i];

						if (player.active && !player.dead && player.Hitbox.Intersects(Projectile.Hitbox))
						{
							SoundEngine.PlaySound(SoundID.Item86.WithVolumeScale(0.7f).WithPitchOffset(0.6f), Projectile.Center);
							player.HealMe(Heal);
							Projectile.Kill();
							break;
						}
					}
				}
			}
		}
	}
}
