using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using WebmilioCommons.Tinq;

namespace RiskOfSlimeRain.Projectiles
{
	public class PanicMinesProj : ModProjectile, IExcludeOnHit
	{
		public bool activate = false;
		public byte timer = 25;
		public byte addRadiusX = 50;
		public byte addRadiusY = 30;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Panic Mines");
			Main.projFrames[projectile.type] = 4;
		}

		public override void SetDefaults()
		{
			base.SetDefaults();
			projectile.Size = new Vector2(14, 36);
			projectile.timeLeft = 3600;
			projectile.friendly = true;
			projectile.penetrate = -1;
			drawOriginOffsetY = 8;
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

		public override void AI()
		{
			projectile.velocity.Y = 10f;
			projectile.LoopAnimation(4);
			Main.npc.WhereActive(n => n.CanBeChasedBy() && n.Hitbox.Intersects(projectile.Hitbox)).Do(n =>
			{
				activate = true;
			});

			if (timer % 5 == 0 && activate) Main.PlaySound(42, (int)projectile.Center.X, (int)projectile.Center.Y, 166, 0.8f, 0.6f);
			if (activate) timer--;
			if (timer == 0)
			{
				Projectile.NewProjectile(projectile.position, new Vector2(0, 0), ModContent.ProjectileType<PanicMinesExplosionProj>(), 0, 0, Main.myPlayer);
				projectile.Kill();
			}
		}

		public override void Kill(int timeLeft)
		{
			Rectangle explosionArea = projectile.Hitbox;
			explosionArea.Inflate(addRadiusX / 2, addRadiusY / 2);
			Main.npc.WhereActive(n => n.CanBeChasedBy() && n.Hitbox.Intersects(explosionArea)).Do(n =>
			{
				n.StrikeNPC((int)projectile.ai[0], 0, 0);
			});
			Main.PlaySound(SoundID.DD2_ExplosiveTrapExplode);
		}
		public override Color? GetAlpha(Color lightColor)
		{
			return Color.White;
		}
	}
}
