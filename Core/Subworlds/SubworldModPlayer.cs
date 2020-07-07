using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Core.Subworlds
{
	public class SubworldPlayer : ModPlayer
	{
		public override bool CloneNewInstances => false;

		public override void PostUpdateBuffs()
		{
			if (SubworldManager.AnyActive() ?? false)
			{
				if (SubworldManager.Current == null)
				{
					SubworldManager.Current = new SubworldMonitor();
				}
				SubworldManager.Current.Update();

				player.noBuilding = true;
				player.AddBuff(BuffID.NoBuilding, 3);
			}
			else
			{
				SubworldManager.Reset();
			}
		}
	}
}
