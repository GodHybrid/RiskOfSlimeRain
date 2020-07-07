using Terraria.ModLoader;

namespace RiskOfSlimeRain.Core.Subworlds
{
	public class SubworldGlobalWall : GlobalWall
	{
		public override bool CanExplode(int i, int j, int type)
		{
			if (SubworldManager.AnyActive() ?? false) return false;
			return base.CanExplode(i, j, type);
		}
	}
}
