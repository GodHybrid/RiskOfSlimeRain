using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Data.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Projectiles;
using Terraria;
using Terraria.ModLoader;
using Terraria.World.Generation;

namespace RiskOfSlimeRain.Data.ROREffects.Common
{
	public class BustlingFungusEffect : RORCommonEffect, IPostUpdateEquips
	{
		const int noMoveTimerMax = 120;
		const float increase = 0.045f;

		public override string Description => $"After {noMoveTimerMax / 60} seconds, heal for {increase.ToPercent()} of your max HP every second";

		public override string FlavorText => "The strongest biological healing agent...\n...is a mushroom";

		public void PostUpdateEquips(Player player)
		{
			int type = ModContent.ProjectileType<BustlingFungusProj>();
			if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[type] < 1 && player.GetRORPlayer().NoInputTimer > noMoveTimerMax)
			{
				Vector2 position = player.Center;
				while (!WorldUtils.Find(position.ToTileCoordinates(), Searches.Chain(new Searches.Down(1), new GenCondition[]
					{
						new Conditions.IsSolid()
					}), out _))
				{
					position.Y++;
				}
				position.Y -= 12; //half the projectiles height
				int heal = (int)(player.statLifeMax2 * increase * Stack);
				Projectile.NewProjectile(position, Vector2.Zero, type, 0, 0, Main.myPlayer, heal, BustlingFungusProj.TimerMax / 2);
			}
		}
	}
}
