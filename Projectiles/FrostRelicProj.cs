using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects;
using RiskOfSlimeRain.Core.ROREffects.Uncommon;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Dusts;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Network;
using Terraria;
using Terraria.ModLoader;
using WebmilioCommons.Tinq;

namespace RiskOfSlimeRain.Projectiles
{
	public class FrostRelicProj : CirclingMovementProj
	{
		private float maxRadius;
		private int startingAngle;
		private bool growingRad = false;

		public override Vector2 Position => projectile.GetOwner().position;

		private float radius;
		public override float Radius { get; set; }

		private int angle;
		public override int Angle { get; set; }

		private int AttackTimer { get; set; }

		public override void SetStaticDefaults()
		{
			base.SetStaticDefaults();
			DisplayName.SetDefault("Frost Relic Icicle");
		}

		public override void SetDefaults()
		{
			base.SetDefaults();
			//maxRadius = (int)projectile.ai[0];
			//Angle = (int)projectile.ai[1];
		}

		public override void Movement()
		{
			maxRadius = (int)projectile.ai[0];
			startingAngle = (int)projectile.ai[1];

			projectile.Center = Position + new Vector2(Radius, 0).RotatedBy(MathHelper.ToRadians(startingAngle + Angle));
			projectile.velocity = Vector2.Zero;
			projectile.rotation = startingAngle + Angle;

			if (!growingRad)
			{
				if (Radius > 0) Radius -= (int)(maxRadius/15f);
				else growingRad = true;
			}
			else
			{
				if (Radius < maxRadius) Radius += (int)(maxRadius / 15f);
				else growingRad = false;
			}
		}

		public override void OtherAI()
		{
			if (projectile.timeLeft < 5) projectile.timeLeft = 5;

			if (AttackTimer <= 0)
			{
				if (Main.myPlayer == projectile.owner)
				{
					Main.npc.WhereActive(n => n.Hitbox.Intersects(projectile.Hitbox)).DoActive(n => n.StrikeNPC(projectile.damage, projectile.knockBack, 0));
				}
				AttackTimer = 9;
			}
			else AttackTimer--;

			Player player = projectile.GetOwner();
			FrostRelicEffect tmp = ROREffectManager.GetEffectOfType<FrostRelicEffect>(player);
			if (tmp != null && !tmp.IsActive) projectile.Kill();
		}

		public override void Kill(int timeLeft)
		{
			while(projectile.alpha < 255)
			{
				projectile.alpha += AlphaDecrease;
			}
		}
	}
}
