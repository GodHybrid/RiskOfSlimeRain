using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace RiskOfSlimeRain.Items.Armor
{
	[AutoloadEquip(EquipType.Legs)]
	public class CommandoLeggings : ModItem
	{
		public override void SetStaticDefaults()
		{

		}

		public override void SetDefaults()
		{
			item.width = 18;
			item.height = 18;
			item.value = 10000;
			item.rare = ItemRarityID.Green;
			item.vanity = true;
		}
	}
}