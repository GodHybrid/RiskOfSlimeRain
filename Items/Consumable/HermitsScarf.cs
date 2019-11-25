using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable
{
	public class HermitsScarf : RORConsumableItem
	{
		public override void Initialize()
		{
			description = "Allows you to evade attacks with 10% chance";
			flavorText = "This thing survived that horrible day\nIt must be able to survive whatever I have to endure, right?";
		}

		public override bool CanUse(RORPlayer mPlayer)
		{
			return mPlayer.scarfs < 6;
		}

		public override void ApplyEffect(RORPlayer mPlayer)
		{
			mPlayer.scarfs++;
		}

		public override void ResetEffect(RORPlayer mPlayer)
		{
			mPlayer.scarfs = 0;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Silk, 999);
			recipe.AddIngredient(ItemID.SlushBlock, 900);
			recipe.AddIngredient(ItemID.Feather, 350);
			recipe.AddIngredient(ItemID.Gi, 5);
			recipe.AddIngredient(ItemID.TrapsightPotion, 200);

			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}
