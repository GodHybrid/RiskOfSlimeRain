using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable
{
	public class RustyKnife : RORConsumableItem
	{
		public override void Initialize()
		{
			description = "15% chance to cause bleeding";
			flavorText = "Murder weapon, case name ELIAS. Probably a lover's spat?\nThere is still dried blood on the knife, so mark it as biological.";
		}

		public override bool CanUse(RORPlayer mPlayer)
		{
			return mPlayer.rustyKnives < 7;
		}

		public override void ApplyEffect(RORPlayer mPlayer)
		{
			mPlayer.rustyKnives++;
		}

		public override void ResetEffect(RORPlayer mPlayer)
		{
			mPlayer.rustyKnives = 0;
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
