using System;
using System.Collections.Generic;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace RiskOfSlimeRain.Core.ItemSpawning.ModIntegration
{
	/// <summary>
	/// Container class for a boss
	/// </summary>
	public class BossChecklistBossInfo
	{
		internal string key = ""; // unique identifier for an entry
		internal string modSource = "";
		internal LocalizedText displayName = null;

		internal float progression = 0f; // See https://github.com/JavidPack/BossChecklist/blob/master/BossTracker.cs#L13 for vanilla boss values
		internal Func<bool> downed = () => false;

		internal bool isBoss = false;
		internal bool isMiniboss = false;
		internal bool isEvent = false;

		internal List<int> npcIDs = new List<int>(); // Does not include minions, only npcids that count towards the NPC still being alive.
		internal Func<LocalizedText> spawnInfo = null;
		internal List<int> spawnItems = new List<int>();
		internal int treasureBag = 0;
		internal List<DropRateInfo> dropRateInfo = new List<DropRateInfo>();
		internal List<int> loot = new List<int>();
		internal List<int> collectibles = new List<int>();

		public override string ToString()
		{
			return progression + ": " + key;
		}
	}
}
