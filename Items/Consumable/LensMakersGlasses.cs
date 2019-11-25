using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable
{
	public class LensMakersGlasses : RORConsumableItem
	{
		public override void Initialize()
		{
			description = "Increases crit chance by 7%";
			flavorText = "Calibrated for high focal alignment\nShould allow for the precision you were asking for";
		}

		public override bool CanUse(RORPlayer mPlayer)
		{
			return mPlayer.lensMakersGlasses < 14;
		}

		public override void ApplyEffect(RORPlayer mPlayer)
		{
			mPlayer.lensMakersGlasses++;
		}

		public override void ResetEffect(RORPlayer mPlayer)
		{
			mPlayer.lensMakersGlasses = 0;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Lens, 500);
			recipe.AddIngredient(ItemID.BlackLens, 40);
			recipe.AddIngredient(ItemID.Ruby, 300);
			recipe.AddIngredient(ItemID.MeteoriteBrick, 350);
			recipe.AddIngredient(ItemID.CorruptHardenedSand, 250);

			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}
