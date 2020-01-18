using RiskOfSlimeRain.Helpers;
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
		public static readonly string path = "RiskOfSlimeRain.Data.NPCEffects.";

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
			return GetEffectIndexOfName(globalNPC, type.Name);
		}

		public static int GetEffectIndexOfName(RORGlobalNPC globalNPC, string typeName)
		{
			return globalNPC.NPCEffects.FindIndex(e => e.Name.EndsWith(typeName));
		}

		public static void ApplyNPCEffect<T>(NPC npc, int duration, bool broadcast = false) where T : NPCEffect
		{
			ApplyNPCEffect(typeof(T), npc, duration, broadcast);
		}

		public static void ApplyNPCEffect(string typeName, NPC npc, int duration, bool broadcast = false)
		{
			try
			{
				Type type = typeof(NPCEffectManager).Assembly.GetType(path + typeName);
				ApplyNPCEffect(type, npc, duration, broadcast);
			}
			catch
			{
				RiskOfSlimeRainMod.Instance.Logger.Info("ApplyNPCEffect: type " + (path + typeName) + " not valid!");
			}
		}

		//No spam protection when broadcasting, so only do it when it's a "one time" apply
		public static void ApplyNPCEffect(Type type, NPC npc, int duration, bool broadcast = false)
		{
			RORGlobalNPC globalNPC = npc.GetGlobalNPC<RORGlobalNPC>();
			int index = GetEffectIndexOfType(globalNPC, type);
			NPCEffect effect;

			if (index > -1)
			{
				//Effect exists
				effect = globalNPC.NPCEffects[index];
				effect.SetTime(duration);
				GeneralHelper.Print("reset duration");
			}
			else
			{
				//Effect doesn't exist, add one
				effect = NPCEffect.CreateInstance(type, duration);
				GeneralHelper.Print("applied new");
				globalNPC.NPCEffects.Add(effect);
			}

			if (broadcast && Main.netMode != NetmodeID.SinglePlayer)
			{
				NPCEffectPacket.SendPacket(npc, effect);
			}
		}
	}
}
