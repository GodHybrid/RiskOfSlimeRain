using RiskOfSlimeRain.NPCs;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;

namespace RiskOfSlimeRain.Data.NPCEffects
{
	/// <summary>
	/// Responsible for operating on NPCEffects of RORGlobalNPC
	/// </summary>
	public static class NPCEffectManager
	{
		/// <summary>
		/// Decrements each effects' time, and removes it if 0
		/// </summary>
		public static void UpdateStatus(RORGlobalNPC globalNPC)
		{
			List<NPCEffect> toRemove = new List<NPCEffect>();
			foreach (var effect in globalNPC.NPCEffects)
			{
				effect.DecrementTime();
				if (effect.RanOut) toRemove.Add(effect);
			}

			foreach (var effect in toRemove)
			{
				globalNPC.NPCEffects.Remove(effect);
			}
		}

		public static T GetEffectOfType<T>(RORGlobalNPC globalNPC) where T : NPCEffect
		{
			return globalNPC.NPCEffects.FirstOrDefault(e => e is T) as T;
		}

		public static bool HasEffectOfType<T>(RORGlobalNPC globalNPC) where T : NPCEffect
		{
			return GetEffectOfType<T>(globalNPC) != null;
		}

		public static int GetEffectIndexOfType(RORGlobalNPC globalNPC, Type type)
		{
			return globalNPC.NPCEffects.FindIndex(e => e.Name == type.Name);
		}

		public static void ApplyNPCEffect<T>(NPC npc, int duration) where T : NPCEffect
		{
			ApplyNPCEffect(typeof(T), npc, duration);
		}

		public static void ApplyNPCEffect(Type type, NPC npc, int duration, bool noBroadcast = false)
		{
			RORGlobalNPC globalNPC = npc.GetGlobalNPC<RORGlobalNPC>();
			int index = GetEffectIndexOfType(globalNPC, type);
			if (index > -1)
			{
				//Effect exists
				globalNPC.NPCEffects[index].SetTime(duration);
			}
			else
			{
				//Effect doesn't exist, add one
				NPCEffect newEffect = NPCEffect.CreateInstance(type, duration);
				globalNPC.NPCEffects.Add(newEffect);
			}

			if (!noBroadcast && Main.netMode != NetmodeID.SinglePlayer)
			{
				//TODO syncing
				//PlayerHealPacket.SendPacket((byte)player.whoAmI, heal);
			}
		}
	}
}
