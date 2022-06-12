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

		/// <summary>
		/// Update localRORPlayer
		/// </summary>
		public static void SetLocalRORPlayer(RORPlayer mPlayer)
		{
			if (Main.myPlayer == mPlayer.player.whoAmI)
			{
				localRORPlayer = mPlayer;
			}
		}

		public static RORPlayer GetRORPlayer(this Player player)
		{
			if (!Main.gameMenu && player.whoAmI == Main.myPlayer && localRORPlayer != null)
			{
				return localRORPlayer;
			}
			return player.GetModPlayer<RORPlayer>();
		}

		/// <summary>
		/// Returns the damage of the players held item. Respects ROR-Mode
		/// </summary>
		public static int GetDamage(this Player player)
		{
			//TODO include ror mode check here
			return Math.Max(player.GetWeaponDamage(player.HeldItem), (int)MinimalDamageProgressive());
		}

		/// <summary>
		/// Base damage the effect deals when no weapon is held or when the weapon is too weak for the current stage.
		/// <br>Increases as the player progresses through the game by defeating bosses.</br>
		/// </summary>
		public static float MinimalDamageProgressive()
		{
			return !Main.hardMode ? 15f : !NPC.downedMoonlord ? 45f : 80f;
		}

		/// <summary>
		/// Make sure to call this only clientside (== Main.myPlayer check where appropriate)
		/// </summary>
		public static void HealMe(this Player player, int heal, bool noBroadcast = false)
		{
			int clampHeal = Math.Min(heal, player.statLifeMax2 - player.statLife);
			if (clampHeal < 0) //Something wrong
			{
				clampHeal = Math.Min(heal, player.statLifeMax - player.statLife);
				if (clampHeal < 0) //Something wrong again
				{
					clampHeal = 0;
				}
			}
			player.HealEffect(heal, false);
			player.statLife += clampHeal;
			if (!noBroadcast && Main.netMode != NetmodeID.SinglePlayer)
			{
				new PlayerHealPacket((byte)player.whoAmI, heal).Send();
			}
			//NetMessage.SendData(MessageID.SpiritHeal, -1, -1, null, player.whoAmI, clampHeal);
		}

		/// <summary>
		/// Counts current savings of the player (in copper coins)
		/// </summary>
		public static long GetSavings(this Player player)
		{
			long inv = Utils.CoinsCount(out _, player.inventory, new int[]
			{
				58, //Mouse item
				57, //Ammo slots
				56,
				55,
				54
			});
			int[] empty = new int[0];
			long piggy = Utils.CoinsCount(out _, player.bank.item, empty);
			long safe = Utils.CoinsCount(out _, player.bank2.item, empty);
			long forge = Utils.CoinsCount(out _, player.bank3.item, empty);
			return Utils.CoinsCombineStacks(out _, new long[]
			{
				inv,
				piggy,
				safe,
				forge
			});
		}

		public static void Unload()
		{
			localRORPlayer = null;
		}
	}
}
