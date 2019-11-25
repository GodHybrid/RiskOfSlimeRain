using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable
{
	public class MysteriousVial : RORConsumableItem
	{
		public override void Initialize()
		{
			description = "Permanently increases health regeneration by 1.2 health per second";
			flavorText = "Side effects may include itching, rashes, bleeding, sensitivity of skin,\ndry patches, permanent scarring, misaligned bone regrowth, rotting of the...";
		}

		public override void ApplyEffect(RORPlayer mPlayer)
		{
			mPlayer.mysteriousVials++;
		}

		public override void ResetEffect(RORPlayer mPlayer)
		{
			mPlayer.mysteriousVials = 0;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Bottle, 1);
			recipe.AddIngredient(ItemID.RegenerationPotion, 400);
			recipe.AddIngredient(ItemID.NeonTetra, 250);
			recipe.AddIngredient(ItemID.DaybloomSeeds, 600);
			recipe.AddIngredient(ItemID.Damselfish, 200);
			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}
