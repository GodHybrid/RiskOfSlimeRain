using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable
{
	public class BitterRoot : RORConsumableItem
	{
		public override void Initialize()
		{
			description = "Permanently increases maximum life by roughly 8%";
			flavorText = "Biggest. Ginseng. Root. Ever.";
		}

		public override bool CanUse(RORPlayer mPlayer)
		{
			return mPlayer.bitterRootIncrease < mPlayer.player.statLifeMax * 3;
		}

		public override void ApplyEffect(RORPlayer mPlayer)
		{
			Player player = mPlayer.player;
			if (mPlayer.bitterRootIncrease + (int)((player.statLifeMax + mPlayer.bitterRootIncrease) * 0.08f) < player.statLifeMax * 3)
			{
				int increase = (int)((player.statLifeMax + mPlayer.bitterRootIncrease) * 0.08f);
				mPlayer.bitterRootIncrease += increase;
				player.statLifeMax2 += mPlayer.bitterRootIncrease;
				player.statLife += mPlayer.bitterRootIncrease;
				if (Main.myPlayer == player.whoAmI)
				{
					player.HealEffect(increase, true);
				}
			}
			else
			{
				//int increase = 10000 - player.GetModPlayer<RORPlayer>().bitterRootIncrease;
				int increase = (player.statLifeMax * 3) - mPlayer.bitterRootIncrease;
				mPlayer.bitterRootIncrease = (player.statLifeMax * 3);
				player.statLifeMax2 += mPlayer.bitterRootIncrease;
				player.statLife += mPlayer.bitterRootIncrease;
				if (Main.myPlayer == player.whoAmI)
				{
					player.HealEffect(increase, true);
				}
			}
		}

		public override void ResetEffect(RORPlayer mPlayer)
		{
			mPlayer.player.statLifeMax2 -= mPlayer.bitterRootIncrease;
			mPlayer.bitterRoots = 0;
			mPlayer.bitterRootIncrease = 0;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.LifeCrystal, 55);
			recipe.AddIngredient(ItemID.HealingPotion, 250);
			recipe.AddIngredient(ItemID.Salmon, 150);
			recipe.AddIngredient(ItemID.Blinkroot, 300);
			recipe.AddIngredient(ItemID.HoneyBlock, 400);
			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}
