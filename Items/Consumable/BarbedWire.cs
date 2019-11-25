using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable
{
	public class BarbedWire : RORConsumableItem
	{
		public override void Initialize()
		{
			description = "Touching enemies deals 50% of your current damage every second";
			flavorText = "Disclaimer: I, or my company, am not responsible for any bodily harm delivered to...";
		}

		public override void ApplyEffect(RORPlayer mPlayer)
		{
			mPlayer.barbedWires++;
			mPlayer.wireTimer++;
		}

		public override void ResetEffect(RORPlayer mPlayer)
		{
			mPlayer.barbedWires = 0;
			mPlayer.wireTimer = -1;
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
