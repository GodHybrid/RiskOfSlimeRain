using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Core.Subworlds
{
	public class SubworldGlobalWall : GlobalWall
	{
		public override bool CanExplode(int i, int j, int type)
		{
			if (SubworldManager.IsActive(FirstLevelBasic.id) ?? false) return false;
			return base.CanExplode(i, j, type);
		}

		public override void KillWall(int i, int j, int type, ref bool fail)
		{
			if (Main.netMode != NetmodeID.Server && Main.LocalPlayer.controlUseItem)
			{
				if (SubworldManager.IsActive(FirstLevelBasic.id) ?? false)
				{
					fail = true;
				}
			}
		}
	}
}
