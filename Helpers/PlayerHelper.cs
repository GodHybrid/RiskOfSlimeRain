using System;
using Terraria;
using Terraria.ID;

namespace RiskOfSlimeRain.Helpers
{
	public static class PlayerHelper
	{
		/// <summary>
		/// Returns the damage of the players held item. Respects ROR-Mode
		/// </summary>
		public static int GetDamage(this Player player)
		{
			//TODO include ror mode check here
			return player.GetWeaponDamage(player.HeldItem);
		}

		public static RORPlayer GetRORPlayer(this Player player)
		{
			return player.GetModPlayer<RORPlayer>();
		}

		/// <summary>
		/// Make sure to call this only clientside (== Main.myPlayer check where appropriate)
		/// </summary>
		public static void HealMe(this Player player, int heal)
		{
			int clampHeal = Math.Min(heal, player.statLifeMax2 - player.statLife);
			player.HealEffect(heal, false);
			player.statLife += clampHeal;
			NetMessage.SendData(MessageID.SpiritHeal, -1, -1, null, player.whoAmI, clampHeal);
		}
	}
}
