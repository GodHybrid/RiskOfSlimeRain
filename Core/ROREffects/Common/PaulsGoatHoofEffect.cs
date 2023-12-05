using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Projectiles;
using System;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Core.ROREffects.Common
{
	public class PaulsGoatHoofEffect : RORCommonEffect, IPostUpdateRunSpeeds
	{
		//const float Initial = 0.10f;
		//const float Increase = 0.05f;

		int timer = 0;

		const int timerMax = 5;

		public override float Initial => ServerConfig.Instance.OriginalStats ? 0.15f : 0.1f;

		public override float Increase => ServerConfig.Instance.OriginalStats ? 0.05f : 0.02f;

		public override string Name => "Paul's Goat Hoof";

		public override string Description => $"Run {Initial.ToPercent()} faster";

		public override string FlavorText => "A hoof from one of my many goats\nThinking it was cancerous, I went to the doctors and low-and-behold; it was";

		public override string UIInfo()
		{
			return $"Speed increase: {Formula().ToPercent()}";
		}

		public void PostUpdateRunSpeeds(Player player)
		{
			player.maxRunSpeed += player.maxRunSpeed * Formula();
			player.moveSpeed += player.moveSpeed * Formula();
			player.accRunSpeed += 0.5f * Formula();

			//TODO revisit this line due to new values
			//if (((player.controlRight && player.velocity.X < -9f) || (player.controlLeft && player.velocity.X > 9f)) && Stack > 5) player.velocity.X /= 1.3f;

			/*
			 * 	else if (this.controlLeft && this.velocity.X > -this.accRunSpeed && this.dashDelay >= 0)
				if (this.velocity.X < -num && this.velocity.Y == 0f && !this.mount.Active)

				else if (this.controlRight && this.velocity.X < this.accRunSpeed && this.dashDelay >= 0)
				if (this.velocity.X > num && this.velocity.Y == 0f && !this.mount.Active)
			 */

			float horizontal = (player.accRunSpeed + player.maxRunSpeed) / 2f;

			if (Main.myPlayer == player.whoAmI && horizontal > 4f)
			{
				if (player.velocity.Y == 0f && !player.mount.Active && player.dashDelay >= 0)
				{
					if ((player.controlLeft || player.controlRight) && Math.Abs(player.velocity.X) > horizontal - 0.5f && timer++ > timerMax)
					{
						timer = 0;
						Vector2 position = new Vector2(player.position.X + (player.direction > 0 ? 0 : player.width), player.position.Y + player.gfxOffY + Main.rand.Next(player.height));
						Projectile.NewProjectile(GetEntitySource(player), position, Vector2.Zero, ModContent.ProjectileType<PaulsGoatHoofProj>(), 0, 0, Main.myPlayer);
					}
				}
			}
		}
	}
}
