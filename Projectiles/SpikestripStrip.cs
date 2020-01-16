using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Buffs;
using Terraria;
using Terraria.ModLoader;
using WebmilioCommons.Tinq;

namespace RiskOfSlimeRain.Projectiles
{
	//ai0 is used to set timeLeft
	public class SpikestripStrip : ModProjectile
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

			projectile.velocity.Y = 10f;
			Main.npc.WhereActive(n => n.Hitbox.Intersects(projectile.Hitbox)).Do(n => n.AddBuff(ModContent.BuffType<SpikestripSlowdown>(), 60));
		}
	}
}
