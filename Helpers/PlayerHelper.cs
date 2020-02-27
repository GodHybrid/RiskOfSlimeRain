using RiskOfSlimeRain.Network;
using System;
using Terraria;
using Terraria.ID;

namespace RiskOfSlimeRain.Helpers
{
	public static class PlayerHelper
	{
		/// <summary>
		/// Cached RORPlayer of Main.LocalPlayer 
		/// </summary>
		private static RORPlayer localRORPlayer;
		private static Player lastLocalPlayer; //Needed so there will be no mismatch of localRORPlayer in the player select menu

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
			if (player.whoAmI == Main.myPlayer)
			{
				if (lastLocalPlayer == null)
				{
					lastLocalPlayer = player;
				}

				if (localRORPlayer == null || lastLocalPlayer != player)
				{
					localRORPlayer = player.GetModPlayer<RORPlayer>();
				}
				lastLocalPlayer = player;

				return localRORPlayer;
			}
			return player.GetModPlayer<RORPlayer>();
		}

		/// <summary>
		/// Make sure to call this only clientside (== Main.myPlayer check where appropriate)
		/// </summary>
		public static void HealMe(this Player player, int heal, bool noBroadcast = false)
		{
			int clampHeal = Math.Min(heal, player.statLifeMax2 - player.statLife);
			player.HealEffect(heal, false);
			player.statLife += clampHeal;
			if (!noBroadcast && Main.netMode != NetmodeID.SinglePlayer)
			{
				new PlayerHealPacket((byte)player.whoAmI, heal).Send();
			}
			//NetMessage.SendData(MessageID.SpiritHeal, -1, -1, null, player.whoAmI, clampHeal);
		}

		public static void Unload()
		{
			localRORPlayer = null;
			lastLocalPlayer = null;
		}
	}
}
