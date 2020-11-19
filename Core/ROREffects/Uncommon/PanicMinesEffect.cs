using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Projectiles;
using Terraria;
using Terraria.ModLoader;
using Terraria.World.Generation;

namespace RiskOfSlimeRain.Core.ROREffects.Uncommon
{
	public class PanicMinesEffect : RORUncommonEffect, IPostHurt
	{
		public override float Initial => 1;

		public override float Increase => 1;

		public const float dmg = 5f;

		public float Dmg => ServerConfig.Instance.OriginalStats ? dmg : dmg - 2f;

		public const float damageThreshold = 0.15f;

		public int MinesDropped => (int)Formula();

		public override string Description => $"Drop a mine at low health for {Dmg.ToPercent()} damage";

		public override string FlavorText => "Must be strapped onto vehicles, NOT personnel!\nIncludes smart-fire, but leave the blast radius regardless. The laws of physics don't pick sides.";

		public override string UIInfo()
		{
			return $"Mine count: {(int)Formula()}";
		}

		public void PostHurt(Player player, bool pvp, bool quiet, double damage, int hitDirection, bool crit)
		{
			if (Main.myPlayer == player.whoAmI && damage >= 50 || player.statLife <= (int)(player.statLifeMax2 * damageThreshold))
			{
				Vector2 position = player.Center;
				int type = ModContent.ProjectileType<PanicMinesProj>();
				int spawnDamage = (int)(Dmg * player.GetDamage());
				for (int i = MinesDropped; i > 0; i--)
				{
					if (!WorldUtils.Find(new Point(position.ToTileCoordinates().X + MinesDropped / 2 - (i - 1), position.ToTileCoordinates().Y + 1), Searches.Chain(new Searches.Right(1), new GenCondition[]
					{
						new Conditions.IsSolid()
					}), out _))
					{
						Projectile.NewProjectile(new Vector2(position.X + ((MinesDropped / 2) - (i - 1)) * 24, position.Y + 1), Vector2.Zero,
													type, 0, 0, Main.myPlayer, spawnDamage);
					}
					else Projectile.NewProjectile(player.Center, Vector2.Zero, type, 0, 0, Main.myPlayer, spawnDamage);
				}
			}
		}
	}
}
