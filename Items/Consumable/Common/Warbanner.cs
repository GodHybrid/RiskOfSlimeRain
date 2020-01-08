using RiskOfSlimeRain.Effects.Common;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable.Common
{
	public class Warbanner : RORConsumableItem<WarbannerEffect>
	{
		//public override void Initialize()
		//{
		//	description = "Chance to drop an empowering banner";
		//	flavorText = "Very very valuable\nDon't drop it; it's worth more than you";
		//}

		//public override void ApplyEffect(RORPlayer mPlayer)
		//{
		//	mPlayer.warbanners++;
		//}

		//public override void ResetEffect(RORPlayer mPlayer)
		//{
		//	mPlayer.warbanners = 0;
		//	RORWorld.pos.Clear();
		//	RORWorld.radius.Clear();
		//}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.FieryGreatsword, 2);
			recipe.AddIngredient(ItemID.HellwingBow, 1);
			recipe.AddIngredient(ItemID.BorealWood, 505);
			recipe.AddIngredient(ItemID.TatteredCloth, 60);
			recipe.AddIngredient(ItemID.InfernoPotion, 50);

			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}
