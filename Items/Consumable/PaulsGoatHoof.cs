using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable
{
	public class PaulsGoatHoof : RORConsumableItem
	{
		public override void Initialize()
		{
			displayName = "Paul's Goat Hoof";
			description = "Run 20% faster";
			flavorText = "A hoof from one of my many goats\nThinking it was cancerous, I went to the doctors and low-and-behold; it was";
		}

		public override void ApplyEffect(RORPlayer mPlayer)
		{
			mPlayer.paulsGoatHooves++;
		}

		public override void ResetEffect(RORPlayer mPlayer)
		{
			mPlayer.paulsGoatHooves = 0;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Bone, 999);
			recipe.AddIngredient(ItemID.FossilOre, 400);
			recipe.AddRecipeGroup("RoR:FastBoots", 8);
			recipe.AddIngredient(ItemID.AsphaltBlock, 350);
			recipe.AddIngredient(ItemID.Rally, 2);

			recipe.SetResult(this, 1);
			recipe.AddRecipe();
		}
	}
}
