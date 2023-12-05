using RiskOfSlimeRain.Core.ROREffects.Common;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable.Common
{
	public class MonsterTooth : RORConsumableItem<MonsterToothEffect>
	{
		public override void SafeAddRecipes()
		{
			Recipe recipe = CreateRecipe(1);
			recipe.AddIngredient(ItemID.Bone, 400);
			recipe.AddIngredient(ItemID.BandofRegeneration, 4);
			recipe.AddIngredient(ItemID.SharkToothNecklace, 2);
			recipe.AddIngredient(ItemID.ChainKnife, 2);
			recipe.Register();
		}
	}
}
