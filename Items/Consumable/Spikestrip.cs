using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable
{
	public class Spikestrip : RORConsumableItem
	{
		public override void Initialize()
		{
			description = "Drop spikestrips on hit, slowing enemies by 20%";
			flavorText = "The doctors say I don't have much time left\nSince you're in the force now and all, I felt obligated to return it to you";
		}

		public override void ApplyEffect(RORPlayer mPlayer)
		{
			mPlayer.spikestrips++;
		}

		public override void ResetEffect(RORPlayer mPlayer)
		{
			mPlayer.spikestrips = 0;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddRecipeGroup("IronBar", 200);
			recipe.AddIngredient(ItemID.Gel, 700);
			recipe.AddIngredient(ItemID.GrayPressurePlate, 120);
			recipe.AddIngredient(ItemID.SnowBrick, 550);
			recipe.AddIngredient(ItemID.CactusSword, 30);

			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}
