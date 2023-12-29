using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Projectiles;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace RiskOfSlimeRain.Core.ROREffects.Common
{
	public class SpikestripEffect : RORCommonEffect, IPostHurt
	{
		//const int Initial = 60;
		//const int Increase = 60;
		//Effect takes place in the RORGlobalNPC with different values
		public const float slow = 0.2f;
		public const int countDropped = 3;

		public override float Initial => 2f;

		public override float Increase => 1f;

		public override LocalizedText Description => base.Description.WithFormatArgs(slow.ToPercent());

		public override string UIInfo()
		{
			return UIInfoText.Format(Formula());
		}

		public void PostHurt(Player player, Player.HurtInfo info)
		{
			if (Main.myPlayer != player.whoAmI) return;
			//for (int i = -1; i < countDropped - 1; i++)
			//{
			//	Projectile.NewProjectile(GetEntitySource(player), player.Bottom + new Vector2(0, -6), new Vector2(2 * i, 0), ModContent.ProjectileType<SpikestripProj>(), 0, 0, Main.myPlayer, (int)Formula() * 60);
			//}

			Vector2 position = player.Center;
			int type = ModContent.ProjectileType<SpikestripProj>();
			int lifetime = (int)Formula() * 60;
			for (int i = countDropped; i > 0; i--)
			{
				if (!WorldUtils.Find(new Point(position.ToTileCoordinates().X + countDropped / 2 - (i - 1), position.ToTileCoordinates().Y + 1), Searches.Chain(new Searches.Right(1), new GenCondition[]
				{
						new Conditions.IsSolid()
				}), out _))
				{
					Projectile.NewProjectile(GetEntitySource(player), new Vector2(position.X + ((countDropped / 2) - (i - 1)) * 36, position.Y + 1), Vector2.Zero,
												type, 0, 0, Main.myPlayer, lifetime);
				}
				else Projectile.NewProjectile(GetEntitySource(player), player.Bottom, Vector2.Zero, type, 0, 0, Main.myPlayer, lifetime);
			}
		}
	}
}
