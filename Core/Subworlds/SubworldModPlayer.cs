using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Core.Subworlds
{
	public class SubworldModPlayer : ModPlayer
	{
		protected override bool CloneNewInstances => false;

		public override void PostUpdateBuffs()
		{
			if (SubworldManager.IsActive(FirstLevelBasic.id) ?? false)
			{
				Player.noBuilding = true;
				Player.AddBuff(BuffID.NoBuilding, 3);
			}
		}
	}
}
