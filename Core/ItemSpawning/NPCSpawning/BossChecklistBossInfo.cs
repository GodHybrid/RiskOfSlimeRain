using System;
using System.Collections.Generic;

namespace RiskOfSlimeRain.Core.ItemSpawning.NPCSpawning
{
	/// <summary>
	/// Container class for a boss
	/// </summary>
	public class BossChecklistBossInfo
	{
		internal string key = ""; // equal to "modSource internalName"
		internal string modSource = "";
		internal string internalName = "";
		internal string displayName = "";
		internal float progression = 0f; // See https://github.com/JavidPack/BossChecklist/blob/master/BossTracker.cs#L13 for vanilla boss values
		internal Func<bool> downed = () => false;
		internal bool isBoss = false;
		internal bool isMiniboss = false;
		internal bool isEvent = false;
		internal List<int> npcIDs = new List<int>(); // Does not include minions, only npcids that count towards the NPC still being alive.
		internal List<int> spawnItem = new List<int>();
		internal List<int> loot = new List<int>();
		internal List<int> collection = new List<int>();

		public override string ToString()
		{
			return progression + " " + key;
		}
	}
}
