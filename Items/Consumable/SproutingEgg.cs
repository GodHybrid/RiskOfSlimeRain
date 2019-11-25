using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable
{
	public class SproutingEgg : RORConsumableItem
	{
		public override void Initialize()
		{
			description = "Permanently increases health regeneration by 2.4 health per second when out of combat";
			flavorText = "This egg seems to be somewhere between hatching and dying\nI can't bring it to myself to cook it alive";
		}

		public override void ApplyEffect(RORPlayer mPlayer)
		{
			mPlayer.sproutingEggs++;
		}

		public override void ResetEffect(RORPlayer mPlayer)
		{
			mPlayer.sproutingEggs = 0;
			mPlayer.sproutingEggTimer = -1;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Vine, 100);
			recipe.AddIngredient(ItemID.RottenEgg, 300);
			recipe.AddIngredient(ItemID.Scorpion, 80);
			recipe.AddIngredient(ItemID.StrangeBrew, 300);
			recipe.AddIngredient(ItemID.Seed, 999);
			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}
