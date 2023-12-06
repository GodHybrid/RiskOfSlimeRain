using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Projectiles;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace RiskOfSlimeRain.Core.ROREffects.Uncommon
{
	public class PanicMinesEffect : RORUncommonEffect, IPostHurt
	{
		public override float Initial => 1;

		public override float Increase => 1;

		public const float baseDamage = 5f;
		public const int balanceQuantCap = 8;

		public float Dmg => ServerConfig.Instance.OriginalStats ? baseDamage : baseDamage - 2f + (MinesDropped >= balanceQuantCap ? Formula() - (float)balanceQuantCap : 0);

		public const float lowHealthThreshold = 0.15f;
		public const float damageThreshold = 0.2f;

		public int MinesDropped => ServerConfig.Instance.OriginalStats ? (int)Formula() : (int)MathHelper.Clamp(Formula(), 0f, 8f);

		public override LocalizedText Description => base.Description.WithFormatArgs(Dmg.ToPercent());

		public override string UIInfo()
		{
			return ServerConfig.Instance.OriginalStats ? $"Mine count: {MinesDropped}"
														: $"Mine count: {MinesDropped} " + (MinesDropped >= balanceQuantCap ? "(max)" : "") + $"\nMine damage: {Dmg.ToPercent()}";
		}

		public void PostHurt(Player player, Player.HurtInfo info)
		{
			if (Main.myPlayer == player.whoAmI && (info.Damage >= (int)(player.statLifeMax2 * damageThreshold) || player.statLife <= (int)(player.statLifeMax2 * lowHealthThreshold)))
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
						Projectile.NewProjectile(GetEntitySource(player), new Vector2(position.X + ((MinesDropped / 2) - (i - 1)) * 24, position.Y + 1), Vector2.Zero,
													type, 0, 0, Main.myPlayer, spawnDamage);
					}
					else Projectile.NewProjectile(GetEntitySource(player), player.Center, Vector2.Zero, type, 0, 0, Main.myPlayer, spawnDamage);
				}
			}
		}
	}
}
