using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Items.Consumable
{
	public class MonsterTooth : RORConsumableItem
	{
		public override void Initialize()
		{
			description = "Killing an enemy will heal you for 10 health";
			flavorText = "Sometimes I felt like it helped me on hunts, ya know?\nLike... instincts";
		}

		public override void ApplyEffect(RORPlayer mPlayer)
		{
			mPlayer.monsterTeeth++;
		}

		public override void ResetEffect(RORPlayer mPlayer)
		{
			mPlayer.monsterTeeth = 0;
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
