using System;

namespace RiskOfSlimeRain.Helpers
{
	public static class NumberHelper
	{
		public static string ToPercent(this float percent, int additionalDecimals = 1)
		{
			if (percent < 0.000001f) return "0%";
			double d = (double)percent * 100;
			int steps = 0;
			while (d < 1)
			{
				steps++;
				d *= 10;
			}
			if (steps > 0)
			{
				d /= Math.Pow(10, steps);
			}
			d = Math.Round(d, steps + additionalDecimals);
			return d.ToString() + "%";
		}
	}
}
