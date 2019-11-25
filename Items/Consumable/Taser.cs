using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable
{
	public class Taser : RORConsumableItem
	{
		public override void Initialize()
		{
			description = "7% chance to snare enemies for 1.5 seconds";
			flavorText = "You say you can fix 'em?\nThese tasers are very very faulty";
		}

		public override void ApplyEffect(RORPlayer mPlayer)
		{
			mPlayer.tasers++;
		}

		public override void ResetEffect(RORPlayer mPlayer)
		{
			mPlayer.tasers = 0;
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
