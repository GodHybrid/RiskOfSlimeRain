﻿using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Projectiles
{
	/// <summary>
	/// Entirely audio focused projectile
	/// </summary>
	public class LensmakersGlassesProj : ModProjectile
	{
		public override string Texture => "RiskOfSlimeRain/Empty";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Lens-Maker's Glasses");
		}

		public override void SetDefaults()
		{
			projectile.Size = new Vector2(2);
			projectile.aiStyle = -1;
			projectile.friendly = true;
			projectile.melee = true;
			projectile.penetrate = -1;
			projectile.hide = true;
			projectile.timeLeft = 3;
			projectile.ignoreWater = true;
			projectile.tileCollide = false;
		}

		public override void AI()
		{
			if (projectile.localAI[0] != 1f)
			{
				Main.PlaySound(SoundID.Shatter, (int)projectile.Center.X, (int)projectile.Center.Y, -1, 1f, 1f);
				projectile.localAI[0] = 1f;
			}
		}
	}
}
