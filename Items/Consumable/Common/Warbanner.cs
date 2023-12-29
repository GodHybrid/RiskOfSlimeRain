using RiskOfSlimeRain.Core.ROREffects.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable.Common
{
	public class Warbanner : RORConsumableItem<WarbannerEffect>
	{
		public override void SafeAddRecipes()
		{
			Recipe recipe = CreateRecipe(1);
			recipe.AddIngredient(ItemID.FieryGreatsword, 2);
			recipe.AddIngredient(ItemID.BorealWood, 505);
			recipe.AddIngredient(ItemID.TatteredCloth, 60);
			recipe.AddIngredient(ItemID.InfernoPotion, 9);
			recipe.Register();
		}
	}
}
