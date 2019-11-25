using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable
{
	public class Medkit : RORConsumableItem
	{
		public override void Initialize()
		{
			description = "Heal for 10 health 1.1 seconds after receiving damage";
			flavorText = "Each Medkit should contain bandages, sterile dressings, soap,\nantiseptics, saline, gloves, scissors, aspirin, codeine, and an Epipen";
		}

		public override void ApplyEffect(RORPlayer mPlayer)
		{
			mPlayer.medkits++;
		}

		public override void ResetEffect(RORPlayer mPlayer)
		{
			mPlayer.medkits = 0;
			mPlayer.medkitTimer = -1;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Safe, 5);
			recipe.AddIngredient(ItemID.Daybloom, 650);
			recipe.AddIngredient(ItemID.BottledHoney, 900);
			recipe.AddIngredient(ItemID.Sake, 1998);
			recipe.AddIngredient(ItemID.Bezoar, 10);
			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}
