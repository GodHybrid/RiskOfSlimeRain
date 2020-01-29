using RiskOfSlimeRain.Data.ROREffects.Interfaces;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader.IO;

namespace RiskOfSlimeRain.Data.ROREffects.Common
{
	public class LifeSavingsEffect : RORCommonEffect, IPostUpdateEquips, IProcessTriggers
	{
		bool justOpenedInventory = false;
		const int interval = 180;
		int timer = interval;

		const int amount = 10;
		int savings = 0;

		string nextMoneyWithdrawn = "0";

		string lastWithdrawn = "0";

		int total = 0;

		string totalText = "0";

		public override void PopulateTag(TagCompound tag)
		{
			tag.Add("total", total);
		}

		public override void PopulateFromTag(TagCompound tag)
		{
			total = tag.GetInt("total");
			totalText = MoneyToString(total);
		}

		public override string Description => $"Generate {amount} copper every {interval / 60} seconds. Withdraw by opening the inventory";

		public override string FlavorText => "hi im billy and heer is money for mom thanks";

		public override string UIInfo => GetUIInfo();

		public void PostUpdateEquips(Player player)
		{
			if (Main.myPlayer != player.whoAmI) return;
			timer--;
			if (timer < 0)
			{
				timer = interval / Stack + 1;
				savings++;
				nextMoneyWithdrawn = MoneyToString(amount * savings);
			}

			if (justOpenedInventory && savings > 0)
			{
				Main.PlaySound(SoundID.CoinPickup);
				lastWithdrawn = MoneyToString(amount * savings);
				//*5 cause sell/buy value stuff
				player.SellItem(amount * 5, savings);
				total += amount * savings;
				totalText = MoneyToString(total);
				savings = 0;
				nextMoneyWithdrawn = MoneyToString(0);
			}
		}

		public void ProcessTriggers(Player player, TriggersSet triggersSet)
		{
			justOpenedInventory = PlayerInput.Triggers.JustPressed.Inventory && !Main.playerInventory;
		}

		private string GetUIInfo()
		{
			string text = string.Empty;
			if (Main.playerInventory)
			{
				text += $"Last withdrawn: {lastWithdrawn}";
			}
			else
			{
				text += $"Next withdrawal: {nextMoneyWithdrawn}";
			}
			return text + $"\nTotal money generated: {totalText}";
		}

		private string MoneyToString(int amount)
		{
			string text = "";
			string[] currencies = new string[] { Language.GetTextValue("Currency.Platinum"), Language.GetTextValue("Currency.Gold"), Language.GetTextValue("Currency.Silver"), Language.GetTextValue("Currency.Copper") };

			if (amount < 1)
			{
				amount = 0;
				//num = 1;
			}

			int cutoff = 1000000;
			int money;
			for (int i = 0; i < 4; i++)
			{
				//from platinum to copper
				money = 0;
				if (i == 3)
				{
					//copper special cause 0 and stuff
					if (amount >= 0)
					{
						money = amount;
					}
				}
				else
				{
					if (amount >= cutoff)
					{
						money = amount / cutoff;
						amount -= money * cutoff;
					}

					cutoff /= 100;
				}

				if (money > 0)
				{
					text += money + " " + currencies[i] + " ";
				}
			}
			if (text == string.Empty) return "0 " + currencies[3];
			return text;
		}
	}
}
