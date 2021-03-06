﻿using Terraria;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Core.Subworlds
{
	public class SubworldGlobalNPC : GlobalNPC
	{
		public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
		{
			if (SubworldManager.IsActive(FirstLevelBasic.id) ?? false) maxSpawns = 0;
		}
	}
}
