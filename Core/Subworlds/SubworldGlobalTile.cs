using Terraria.ModLoader;

namespace RiskOfSlimeRain.Core.Subworlds
{
	public class SubworldGlobalTile : GlobalTile
	{
		public override bool CanKillTile(int i, int j, int type, ref bool blockDamaged)
		{
			if (SubworldManager.AnyActive() ?? false) return false;
			return base.CanKillTile(i, j, type, ref blockDamaged);
		}

		public override bool CanExplode(int i, int j, int type)
		{
			if (SubworldManager.AnyActive() ?? false) return false;
			return base.CanExplode(i, j, type);
		}
	}
}
