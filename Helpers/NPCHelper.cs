using System;
using Terraria;

namespace RiskOfSlimeRain.Helpers
{
	public static class NPCHelper
	{
		/// <summary>
		/// Heals the NPC, no netmode check required, make sure to call this on all sides
		/// </summary>
		public static void HealMe(this NPC npc, int heal)
		{
			int clampHeal = Math.Min(heal, npc.lifeMax - npc.life);
			npc.HealEffect(heal);
			npc.life += clampHeal;
		}
	}
}
