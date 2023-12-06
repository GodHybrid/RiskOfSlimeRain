using RiskOfSlimeRain.Network;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

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
			if (!mPlayer.Player.isDisplayDollOrInanimate)
			{
				localRORPlayer = mPlayer;
			}
		}

		public static RORPlayer GetRORPlayer(this Player player)
		{
			//TODO 1.4.4 same fix as thorium with the dummy
			if (!Main.gameMenu && player.whoAmI == Main.myPlayer && !player.isDisplayDollOrInanimate && localRORPlayer != null)
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

		/// <summary>
		/// Gives coins to the player
		/// </summary>
		public static bool GiveCoinsToPlayer(this Player player, int amount)
		{
			if (amount <= 0)
				return false;

			Item[] array = new Item[58];
			for (int i = 0; i < 58; i++)
			{
				array[i] = new Item();
				array[i] = player.inventory[i].Clone();
			}

			long num = amount;
			if (num < 1)
				num = 1L;

			bool flag = false;
			while (num >= 1000000 && !flag)
			{
				int num3 = -1;
				for (int num4 = 53; num4 >= 0; num4--)
				{
					if (num3 == -1 && (player.inventory[num4].type == 0 || player.inventory[num4].stack == 0))
						num3 = num4;

					while (player.inventory[num4].type == 74 && player.inventory[num4].stack < player.inventory[num4].maxStack && num >= 1000000)
					{
						player.inventory[num4].stack++;
						num -= 1000000;
						player.DoCoins(num4);
						if (player.inventory[num4].stack == 0 && num3 == -1)
							num3 = num4;
					}
				}

				if (num >= 1000000)
				{
					if (num3 == -1)
					{
						flag = true;
						continue;
					}

					player.inventory[num3].SetDefaults(74);
					num -= 1000000;
				}
			}

			while (num >= 10000 && !flag)
			{
				int num5 = -1;
				for (int num6 = 53; num6 >= 0; num6--)
				{
					if (num5 == -1 && (player.inventory[num6].type == 0 || player.inventory[num6].stack == 0))
						num5 = num6;

					while (player.inventory[num6].type == 73 && player.inventory[num6].stack < player.inventory[num6].maxStack && num >= 10000)
					{
						player.inventory[num6].stack++;
						num -= 10000;
						player.DoCoins(num6);
						if (player.inventory[num6].stack == 0 && num5 == -1)
							num5 = num6;
					}
				}

				if (num >= 10000)
				{
					if (num5 == -1)
					{
						flag = true;
						continue;
					}

					player.inventory[num5].SetDefaults(73);
					num -= 10000;
				}
			}

			while (num >= 100 && !flag)
			{
				int num7 = -1;
				for (int num8 = 53; num8 >= 0; num8--)
				{
					if (num7 == -1 && (player.inventory[num8].type == 0 || player.inventory[num8].stack == 0))
						num7 = num8;

					while (player.inventory[num8].type == 72 && player.inventory[num8].stack < player.inventory[num8].maxStack && num >= 100)
					{
						player.inventory[num8].stack++;
						num -= 100;
						player.DoCoins(num8);
						if (player.inventory[num8].stack == 0 && num7 == -1)
							num7 = num8;
					}
				}

				if (num >= 100)
				{
					if (num7 == -1)
					{
						flag = true;
						continue;
					}

					player.inventory[num7].SetDefaults(72);
					num -= 100;
				}
			}

			while (num >= 1 && !flag)
			{
				int num9 = -1;
				for (int num10 = 53; num10 >= 0; num10--)
				{
					if (num9 == -1 && (player.inventory[num10].type == 0 || player.inventory[num10].stack == 0))
						num9 = num10;

					while (player.inventory[num10].type == 71 && player.inventory[num10].stack < player.inventory[num10].maxStack && num >= 1)
					{
						player.inventory[num10].stack++;
						num--;
						player.DoCoins(num10);
						if (player.inventory[num10].stack == 0 && num9 == -1)
							num9 = num10;
					}
				}

				if (num >= 1)
				{
					if (num9 == -1)
					{
						flag = true;
						continue;
					}

					player.inventory[num9].SetDefaults(71);
					num--;
				}
			}

			if (flag)
			{
				for (int j = 0; j < 58; j++)
				{
					player.inventory[j] = array[j].Clone();
				}

				return false;
			}

			return true;
		}

		public static void ApplyDamageToNPC_ProcHeldItem(this Player player, NPC target, int damage, float knockback = 0f, int direction = 0, bool crit = false, DamageClass damageType = null)
		{
			player.ApplyDamageToNPC(target, damage, knockback, direction, crit, damageType); //Procs ModPlayer.OnHitNPC (which we can't use) but not the item/projectile variants

			var item = player.HeldItem;
			if (item?.damage > 0)
			{
				//Copied vanilla code to calculate hitInfo to feed into OnHitNPCWithItem
				var modifiers = target.GetIncomingStrikeModifiers(damageType ?? DamageClass.Default, 0);
				PlayerLoader.ModifyHitNPC(player, target, ref modifiers);

				player.ApplyBannerOffenseBuff(target, ref modifiers);

				modifiers.ArmorPenetration += player.GetTotalArmorPenetration(damageType ?? DamageClass.Generic);

				var hitInfo = modifiers.ToHitInfo(damage, crit, knockback, false, player.luck);
				PlayerLoader.OnHitNPCWithItem(player, item, target, hitInfo, damage); //damage is not accurate here but we don't have access to damageDone
			}
		}

		public static void Unload()
		{
			localRORPlayer = null;
		}
	}
}
