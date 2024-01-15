using RiskOfSlimeRain.Core.ROREffects.Interfaces;
using RiskOfSlimeRain.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
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

		public override LocalizedText Description => base.Description.WithFormatArgs(amount, interval / 60);

		public static LocalizedText UIInfoNextText { get; private set; }
		public static LocalizedText UIInfoTotalText { get; private set; }

		public override void SetStaticDefaults()
		{
			UIInfoNextText ??= GetLocalization("UIInfoNext");
			UIInfoTotalText ??= GetLocalization("UIInfoTotal");
		}

		public override string UIInfo()
		{
			string text = string.Empty;
			if (Main.playerInventory)
			{
				text += UIInfoText.Format(lastWithdrawn);
			}
			else
			{
				text += UIInfoNextText.Format(nextMoneyWithdrawn);
			}
			return text + $"\n" + UIInfoTotalText.Format(totalText);
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
				SoundEngine.PlaySound(SoundID.CoinPickup);
				int money = amount * savings;
				lastWithdrawn = money.MoneyToString();
				player.GiveCoinsToPlayer(money);
				total += money;
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
