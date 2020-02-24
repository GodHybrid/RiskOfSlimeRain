using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Projectiles;
using Terraria;
using Terraria.ModLoader;
using Terraria.World.Generation;

namespace RiskOfSlimeRain.Core.ROREffects.Uncommon
{
	//TODO Makes mines drop
	public class PanicMinesEffect : RORUncommonEffect, IPostHurt
	{
		public const int initial = 0;
		public const int increase = 1;
		public const float dmg = 5f;

		public int MinesDropped => initial + increase * Stack;

		public override string Description => $"Drop a mine at low health for {dmg.ToPercent()} damage.";
		public override string FlavorText => "Must be strapped onto vehicles, NOT personnel!\nIncludes smart-fire, but leave the blast radius regardless. The laws of physics don't pick sides.";

		public void PostHurt(Player player, bool pvp, bool quiet, double damage, int hitDirection, bool crit)
		{
			Vector2 position = player.position;
			if (damage >= 50 || player.statLife <= (int)(player.statLifeMax2 * 0.15f))
			{
				for (int i = MinesDropped; i > 0; i--)
				{
					if (!WorldUtils.Find(new Point(position.ToTileCoordinates().X + (int)(MinesDropped / 2) - (i - 1), position.ToTileCoordinates().Y + 1), Searches.Chain(new Searches.Right(1), new GenCondition[]
					{
						new Conditions.IsSolid()
					}), out _))
					{
						Projectile.NewProjectile(new Vector2(position.X + ((MinesDropped / 2) - (i - 1)) * 16, position.Y + 1), new Vector2(0, 0),
													ModContent.ProjectileType<PanicMinesProj>(), 0, 0, Main.myPlayer, (int)(dmg * player.GetDamage()));
					}
					else Projectile.NewProjectile(player.position, new Vector2(0, 0), ModContent.ProjectileType<PanicMinesProj>(), 0, 0, Main.myPlayer, (int)(dmg * player.GetDamage()));
				}
			}
		}
	}
}
