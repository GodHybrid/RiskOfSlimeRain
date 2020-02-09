using System;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

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

		/// <summary>
		/// For using it like this: recipe.AddRecipeGroupID(RecipeGroupID.IronBar, 10);
		/// </summary>
		public static void AddRecipeGroupID(this ModRecipe recipe, int id, int stack)
		{
			string groupName = RecipeGroup.recipeGroupIDs.FirstOrDefault(x => x.Value == id).Key;
			recipe.AddRecipeGroup(groupName, stack);
		}
	}
}
