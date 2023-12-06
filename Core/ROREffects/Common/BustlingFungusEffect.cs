using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Projectiles;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace RiskOfSlimeRain.Core.ROREffects.Common
{
	public class BustlingFungusEffect : RORCommonEffect, IPostUpdateEquips
	{
		private int NoMoveTimerMax => ServerConfig.Instance.OriginalStats ? 120 : 240;
		//const float Increase = 0.045f;

		public override float Initial => ServerConfig.Instance.OriginalStats ? 0.045f : 0.02f;

		public override float Increase => ServerConfig.Instance.OriginalStats ? 0.045f : 0.02f;

		public override LocalizedText Description => base.Description.WithFormatArgs(NoMoveTimerMax / 60, Increase.ToPercent());

		public override string UIInfo()
		{
			return "Requires solid ground below you to spawn" +
				$"\nHeal amount: {GetIncreaseAmount(Player)}";
		}

		public int GetIncreaseAmount(Player player)
		{
			return (int)Math.Floor(player.statLifeMax2 * Formula()) + 1;
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
					position.Y -= BustlingFungusProj.Height / 2; //Half the projectiles height
					int heal = GetIncreaseAmount(player);
					Projectile.NewProjectile(GetEntitySource(player), position, Vector2.Zero, type, 0, 0, Main.myPlayer, heal, BustlingFungusProj.TimerMax / 2);
				}
			}
		}
	}
}
