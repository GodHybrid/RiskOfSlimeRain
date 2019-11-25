using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable
{
	public class MortarTube : RORConsumableItem
	{
		public override void Initialize()
		{
			description = "9% chance to fire a mortar for 170% damage";
			flavorText = "You stick explosives down the end, then you fire the explosive.\nI suppose you can beat them with the tube afterwards.";
		}

		public override void ApplyEffect(RORPlayer mPlayer)
		{
			mPlayer.mortarTubes++;
		}

		public override void ResetEffect(RORPlayer mPlayer)
		{
			mPlayer.mortarTubes = 0;
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
