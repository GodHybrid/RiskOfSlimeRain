using RiskOfSlimeRain.Effects.Interfaces;
using RiskOfSlimeRain.Helpers;
using System;
using Terraria;
using Terraria.DataStructures;

namespace RiskOfSlimeRain.Effects.Common
{
	public class HeadstompersEffect : ROREffect, IPostUpdateEquips, IPreHurt
	{
		const float initial = 5.07f;
		const float increase = 0.3f;

		public override string Description => $"Hurt enemies by falling for up to {initial.ToPercent()} damage";

		public override string FlavorText => "Combat Ready Spikeshoes, lovingly named 'Headstompers', allow you to get the drop on foes. \nLiterally. Vertically.";

		public void PostUpdateEquips(Player player)
		{
			player.maxFallSpeed += 6f;
		}

		public bool PreHurt(Player player, bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
		{
			if (player.velocity.Y > 10f && Math.Abs(player.velocity.X) < 15f && damageSource.SourceNPCIndex > -1 && !player.immune)
			{
				//TODO this definitely needs balancing/testing lol
				Main.npc[damageSource.SourceNPCIndex].StrikeNPC((int)(player.GetWeaponDamage(player.HeldItem) * ((initial + (increase * (Stack - 1))) * player.velocity.Y / 16)), 2f, 0, false);
				player.immune = true;
				player.immuneTime = 40;
				return false;
			}
			return true;
		}
	}
}
