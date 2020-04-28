using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.World.Generation;

namespace RiskOfSlimeRain.Core.ROREffects.Common
{
	public class BustlingFungusEffect : RORCommonEffect, IPostUpdateEquips
	{
		private int NoMoveTimerMax => ServerConfig.Instance.RorStats ? 120 : 240;
		//const float Increase = 0.045f;

		public override float Initial => ServerConfig.Instance.RorStats ? 0.045f : 0.02f;

		public override float Increase => ServerConfig.Instance.RorStats ? 0.045f : 0.02f;

		public override string Description => $"After {NoMoveTimerMax / 60} seconds, heal for {Increase.ToPercent()} of your max HP every second";

		public override string FlavorText => "The strongest biological healing agent...\n...is a mushroom";

		public override string UIInfo()
		{
			return "Requires solid ground below you to spawn" +
				$"\nHeal amount: {GetIncreaseAmount(Player)}";
		}

		public int GetIncreaseAmount(Player player)
		{
			return (int)(player.statLifeMax2 * Formula());
		}

		public void PostUpdateEquips(Player player)
		{
			int type = ModContent.ProjectileType<BustlingFungusProj>();
			if (player.whoAmI == Main.myPlayer && Main.netMode != NetmodeID.Server && player.ownedProjectileCounts[type] < 1 && player.GetRORPlayer().NoInputTimer > NoMoveTimerMax)
			{
				Vector2 position = player.Bottom;
				Point p;
				const int maxDistanceInTiles = BustlingFungusProj.Height / 16 + 2;
				if (WorldUtils.Find(position.ToTileCoordinates(), Searches.Chain(new Searches.Down(maxDistanceInTiles), new GenCondition[]
						{
							new Conditions.IsSolid()
						}), out p))
				{
					position = p.ToWorldCoordinates(8f, 0f);
					position.Y -= BustlingFungusProj.Height >> 1; //Half the projectiles height
					int heal = GetIncreaseAmount(player);
					Projectile.NewProjectile(position, Vector2.Zero, type, 0, 0, Main.myPlayer, heal, BustlingFungusProj.TimerMax >> 1);
				}
			}
		}
	}
}
