using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable
{
	public class StickyBomb : RORConsumableItem
	{
		public override void Initialize()
		{
			description = "8% chance to attach a bomb to an enemy, detonating for 140% damage";
			flavorText = "Once you take the wrapping off, the adhesive is ACTIVE. DON'T TOUCH IT.\nYOU STICK THAT END ON BAD THINGS, NOT YOURSELF.";
		}

		public override void ApplyEffect(RORPlayer mPlayer)
		{
			mPlayer.stickyBombs++;
		}

		public override void ResetEffect(RORPlayer mPlayer)
		{
			mPlayer.stickyBombs = 0;
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
