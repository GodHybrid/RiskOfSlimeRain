using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Core.Subworlds
{
	public class SubworldGlobalTile : GlobalTile
	{
		public override bool CanKillTile(int i, int j, int type, ref bool blockDamaged)
		{
			if (SubworldManager.IsActive(FirstLevelBasic.id) ?? false) return false;
			return base.CanKillTile(i, j, type, ref blockDamaged);
		}

		public override bool CanExplode(int i, int j, int type)
		{
			if (SubworldManager.IsActive(FirstLevelBasic.id) ?? false) return false;
			return base.CanExplode(i, j, type);
		}

		public override bool CanPlace(int i, int j, int type)
		{
			if (SubworldManager.IsActive(FirstLevelBasic.id) ?? false) return false;
			return base.CanPlace(i, j, type);
		}

		public override bool Slope(int i, int j, int type)
		{
			if (SubworldManager.IsActive(FirstLevelBasic.id) ?? false) return false;
			return base.Slope(i, j, type);
		}

		public static void Load()
		{
			On.Terraria.Player.PickTile += Player_PickTile;
		}

		private static void Player_PickTile(On.Terraria.Player.orig_PickTile orig, Player self, int x, int y, int pickPower)
		{
			if (SubworldManager.IsActive(FirstLevelBasic.id) ?? false) return;
			orig(self, x, y, pickPower);
		}
	}
}
