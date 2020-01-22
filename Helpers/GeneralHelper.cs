using System;
using Terraria;
using Terraria.ID;

namespace RiskOfSlimeRain.Helpers
{
	public static class GeneralHelper
	{
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
