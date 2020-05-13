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
	}
}
