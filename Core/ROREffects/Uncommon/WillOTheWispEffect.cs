using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Projectiles;
using System;
using Terraria;
using Terraria.ModLoader;
using Terraria.World.Generation;

namespace RiskOfSlimeRain.Core.ROREffects.Uncommon
{
	public class WillOTheWispEffect : RORUncommonEffect, IOnKill
	{
		Random rnd = new Random();
		public override float Initial => ServerConfig.Instance.OriginalStats ? 5f : 3f;

		public override float Increase => 1f;

		public const float procChance = 0.33f;

		public float Dmg => Formula();
		public float lavaCreationChance => ServerConfig.Instance.OriginalStats ? procChance : procChance - 0.08f;

		public override string Name => "Will-o'-the-Wisp";

		public override string Description => $"{lavaCreationChance.ToPercent()} chance on killing an enemy to create a lava pillar for {Dmg.ToPercent()} damage";

		public override string FlavorText => "Tiny guy; must be a baby. He (she?) doesn't seem to eat anything at all.\nI don't think he can get out of the jar; it hasn't made any attempt to.";

		public override string UIInfo()
		{
			return $"Activation chance: {lavaCreationChance.ToPercent(2)}\nDamage per activation: {Dmg.ToPercent()}";
		}

		//public void PostHurt(Player player, bool pvp, bool quiet, double damage, int hitDirection, bool crit)
		//{
		//	if (Main.myPlayer == player.whoAmI && damage >= 50 || player.statLife <= (int)(player.statLifeMax2 * damageThreshold))
		//	{
		//		Vector2 position = player.Center;
		//		int type = ModContent.ProjectileType<PanicMinesProj>();
		//		int spawnDamage = (int)(Dmg * player.GetDamage());
		//		for (int i = MinesDropped; i > 0; i--)
		//		{
		//			if (!WorldUtils.Find(new Point(position.ToTileCoordinates().X + MinesDropped / 2 - (i - 1), position.ToTileCoordinates().Y + 1), Searches.Chain(new Searches.Right(1), new GenCondition[]
		//			{
		//				new Conditions.IsSolid()
		//			}), out _))
		//			{
		//				Projectile.NewProjectile(new Vector2(position.X + ((MinesDropped / 2) - (i - 1)) * 24, position.Y + 1), Vector2.Zero,
		//											type, 0, 0, Main.myPlayer, spawnDamage);
		//			}
		//			else Projectile.NewProjectile(player.Center, Vector2.Zero, type, 0, 0, Main.myPlayer, spawnDamage);
		//		}
		//	}
		//}

		public void OnKillNPC(Player player, Item item, NPC target, int damage, float knockback, bool crit)
		{
			if(rnd.NextDouble() <= lavaCreationChance) Projectile.NewProjectile(target.Center, Vector2.Zero, ModContent.ProjectileType<WillOTheWispProj>(), 0, 0, Main.myPlayer, (int)(Dmg * player.GetDamage()));
		}

		public void OnKillNPCWithProj(Player player, Projectile proj, NPC target, int damage, float knockback, bool crit)
		{
			if (rnd.NextDouble() <= lavaCreationChance) Projectile.NewProjectile(target.Center, Vector2.Zero, ModContent.ProjectileType<WillOTheWispProj>(), 0, 0, Main.myPlayer, (int)(Dmg * player.GetDamage()));
		}
	}
}
