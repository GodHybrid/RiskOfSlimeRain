using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Projectiles;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Core.ROREffects.Common
{
	public class HeadstompersEffect : RORCommonEffect, IPostUpdateEquips, IFreeDodge
	{
		public const float velocityDecrease = 0.9f;
		//const float Initial = 5.07f;
		//const float Increase = 0.3f;

		public override float Initial => 5.07f;

		public override float Increase => 0.3f;

		public override string Description => $"Hurt enemies by falling for up to {Initial.ToPercent()} damage";

		public override string FlavorText => "Combat Ready Spikeshoes, lovingly named 'Headstompers', allow you to get the drop on foes. \nLiterally. Vertically.";

		public override string UIInfo()
		{
			return $"Damage: {Formula().ToPercent()}";
		}

		public void PostUpdateEquips(Player player)
		{
			player.maxFallSpeed += 6f;
		}

		public bool FreeDodge(Player player, Player.HurtInfo info)
		{
			int whoAmI = info.DamageSource.SourceNPCIndex;
			if (player.velocity.Y > 10f && Math.Abs(player.velocity.X) < 15f && whoAmI > -1)
			{
				player.immune = true;
				player.immuneTime = 5;
				if (Main.myPlayer == player.whoAmI)
				{
					NPC npc = Main.npc[whoAmI];
					int dmg = (int)(player.GetDamage() * (Formula() * player.velocity.Y / 16));
					player.ApplyDamageToNPC(npc, dmg, 2f, 0, false); //Procs ModPlayer.OnHitNPC but not the item/projectile variants
					Projectile.NewProjectile(GetEntitySource(player), npc.Center.X, npc.Bottom.Y - 11f, 0, 0, ModContent.ProjectileType<HeadstompersProj>(), 0, 0, Main.myPlayer, (int)npc.Top.Y, whoAmI);
				}
				return true;
			}
			return false;
		}
	}
}
