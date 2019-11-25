using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable
{
	public class Headstompers : RORConsumableItem
	{
		public override void Initialize()
		{
			description = "Hurt enemies by falling for up to 507% damage";
			flavorText = "Combat Ready Spikeshoes, lovingly named 'Headstompers', allow you to get the drop on foes. \nLiterally. Vertically.";
		}

		public override void ApplyEffect(RORPlayer mPlayer)
		{
			mPlayer.stompers++;
		}

		public override void ResetEffect(RORPlayer mPlayer)
		{
			mPlayer.stompers = 0;
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
