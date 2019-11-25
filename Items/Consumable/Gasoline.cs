using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable
{
	public class Gasoline : RORConsumableItem
	{
		public override void Initialize()
		{
			description = "Killing enemies burns the ground to deal 60% damage and set enemies on fire";
			flavorText = "Gasoline, eh?\nSurprising to find a gas station these days, with everyone drivin' around them electro cars.";
		}

		public override void ApplyEffect(RORPlayer mPlayer)
		{
			mPlayer.gasCanisters++;
		}

		public override void ResetEffect(RORPlayer mPlayer)
		{
			mPlayer.gasCanisters = 0;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Bone, 999);
			recipe.AddIngredient(ItemID.Stinger, 400);
			recipe.AddIngredient(ItemID.SharkToothNecklace, 5);
			recipe.AddIngredient(ItemID.CoralstoneBlock, 55);
			recipe.AddIngredient(ItemID.SharkFin, 250);

			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}
