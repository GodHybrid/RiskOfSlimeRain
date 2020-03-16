using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader.IO;

namespace RiskOfSlimeRain.Core.ROREffects.Common
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
			totalText = total.MoneyToString();
		}

		public override string Description => $"Generate {amount} copper every {interval / 60} seconds. Withdraw by opening the inventory";

		public override string FlavorText => "hi im billy and heer is money for mom thanks";

		public override string UIInfo()
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

		public void PostUpdateEquips(Player player)
		{
			if (Main.netMode == NetmodeID.Server || player.whoAmI != Main.myPlayer) return;
			timer--;
			if (timer < 0)
			{
				timer = interval / Stack + 1;
				savings++;
				nextMoneyWithdrawn = (amount * savings).MoneyToString();
			}

			if (justOpenedInventory && savings > 0)
			{
				Main.PlaySound(SoundID.CoinPickup);
				lastWithdrawn = (amount * savings).MoneyToString();
				//*5 cause sell/buy value stuff
				player.SellItem(amount * 5, savings);
				total += amount * savings;
				totalText = total.MoneyToString();
				savings = 0;
				nextMoneyWithdrawn = 0.MoneyToString();
			}
		}

		public void ProcessTriggers(Player player, TriggersSet triggersSet)
		{
			justOpenedInventory = PlayerInput.Triggers.JustPressed.Inventory && !Main.playerInventory;
		}
	}
}
