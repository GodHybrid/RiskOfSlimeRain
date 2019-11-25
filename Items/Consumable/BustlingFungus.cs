using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable
{
	public class BustlingFungus : RORConsumableItem
	{
		public override void Initialize()
		{
			description = "Grants \"Fungal Defense Mechanism\"\nStand still for 2 seconds to activate the buff\nHeals for 4.5% of your max HP every second";
			flavorText = "The strongest biological healing agent...\n...is a mushroom";
		}

		public override void ApplyEffect(RORPlayer mPlayer)
		{
			mPlayer.bustlingFungi++;
			mPlayer.bustlingFungusHeals++;
			mPlayer.fungalRadius += 16;
			mPlayer.fungalDefense = true;
		}

		public override void ResetEffect(RORPlayer mPlayer)
		{
			mPlayer.bustlingFungi = 0;
			mPlayer.bustlingFungusHeals = 0;
			mPlayer.fungalRadius = 0;
			mPlayer.fungalDefense = false;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.GlowingMushroom, 500);
			recipe.AddIngredient(ItemID.RestorationPotion, 200);
			recipe.AddIngredient(ItemID.JungleSpores, 250);
			recipe.AddIngredient(ItemID.TealMushroom, 50);
			recipe.AddIngredient(ItemID.GreenMushroom, 50);
			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}
