﻿using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using RiskOfSlimeRain.Projectiles;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Core.ROREffects.Common
{
	public class HeadstompersEffect : RORCommonEffect, IPostUpdateEquips, IPreHurt
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

		public bool PreHurt(Player player, bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
		{
			if (player.velocity.Y > 10f && Math.Abs(player.velocity.X) < 15f && damageSource.SourceNPCIndex > -1)
			{
				player.immune = true;
				player.immuneTime = 5;
				if (Main.myPlayer == player.whoAmI)
				{
					NPC npc = Main.npc[damageSource.SourceNPCIndex];
					int dmg = (int)(player.GetDamage() * (Formula() * player.velocity.Y / 16));
					player.ApplyDamageToNPC(npc, dmg, 2f, 0, false);
					Item item = player.HeldItem;
					if (!item.IsAir)
					{
						ItemLoader.OnHitNPC(item, player, npc, dmg, 0f, false);
						NPCLoader.OnHitByItem(npc, player, item, dmg, 0f, false);
						PlayerHooks.OnHitNPC(player, item, npc, dmg, 0f, false);
					}
					Projectile.NewProjectile(npc.Center.X, npc.Bottom.Y - 11f, 0, 0, ModContent.ProjectileType<HeadstompersProj>(), 0, 0, Main.myPlayer, (int)npc.Top.Y, damageSource.SourceNPCIndex);
				}
				return false;
			}
			return true;
		}
	}
}
