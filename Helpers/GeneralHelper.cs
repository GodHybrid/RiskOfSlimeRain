using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;

namespace RiskOfSlimeRain.Helpers
{
	public static class GeneralHelper
	{
		/// <summary>
		/// Converts a float into a string representation as a percentage
		/// </summary>
		public static string ToPercent(this float percent, int additionalDecimals = 1)
		{
			if (percent < 0.000001f) return "0%";
			double d = (double)percent * 100;
			int steps = 0;
			double e = d;
			while (e < 1)
			{
				steps++;
				e *= 10;
			}
			d = Math.Round(d, steps + additionalDecimals);
			return d.ToString() + "%";
		}

		/// <summary>
		/// Converts an integer into a string representing its monetary value
		/// </summary>
		public static string MoneyToString(this int amount)
		{
			return ((long)amount).MoneyToString();
		}

		/// <summary>
		/// Converts a long into a string representing its monetary value
		/// </summary>
		public static string MoneyToString(this long amount)
		{
			string text = "";
			string[] currencies = new string[] { Language.GetTextValue("Currency.Platinum"), Language.GetTextValue("Currency.Gold"), Language.GetTextValue("Currency.Silver"), Language.GetTextValue("Currency.Copper") };

			if (amount < 1)
			{
				amount = 0;
				//num = 1;
			}

			int cutoff = 1000000;
			long money;
			for (int i = 0; i < currencies.Length; i++)
			{
				//From platinum to copper
				money = 0;
				if (i == currencies.Length - 1)
				{
					//Copper special cause 0 and stuff
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

		public static void Print(string message)
		{
			RiskOfSlimeRainMod.Instance.Logger.Info(message);
			if (Main.netMode == NetmodeID.Server)
			{
				Console.WriteLine(message);
			}
			else
			{
				Main.NewText(message);
			}
		}
	}
}
