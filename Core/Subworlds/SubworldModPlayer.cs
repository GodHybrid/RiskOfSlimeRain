using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Core.Subworlds
{
	public class SubworldModPlayer : ModPlayer
	{
		public override bool CloneNewInstances => false;

		public override void PostUpdateBuffs()
		{
			if (SubworldManager.IsActive(FirstLevelBasic.id) ?? false)
			{
				player.noBuilding = true;
				player.AddBuff(BuffID.NoBuilding, 3);
			}
		}
	}
}
