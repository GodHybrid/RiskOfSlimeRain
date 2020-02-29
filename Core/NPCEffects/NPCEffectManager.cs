using RiskOfSlimeRain.Core.Misc;
using RiskOfSlimeRain.Network.Data;
using RiskOfSlimeRain.NPCs;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;

namespace RiskOfSlimeRain.Core.NPCEffects
{
	/// <summary>
	/// Responsible for operating on NPCEffects of RORGlobalNPC
	/// </summary>
	public static class NPCEffectManager
	{
		public static readonly string prefix = "RiskOfSlimeRain.Core.NPCEffects.";
		public static readonly string suffix = "NPCEffect";

		/// <summary>
		/// Type identified by sbyte (0 to 127)
		/// </summary>
		private static List<Type> NPCEffectTypes;

		public static void Load()
		{
			//Reflection shenanigans
			Type[] types = typeof(NPCEffectManager).Assembly.GetTypes();
			NPCEffectTypes = new List<Type>();
			Dictionary<string, string> loadedTypeNamespaceToName = new Dictionary<string, string>();
			foreach (var type in types)
			{
				if (!type.IsAbstract && type.IsSubclassOf(typeof(NPCEffect)))
				{
					if (!(type.FullName.StartsWith(prefix) && type.FullName.EndsWith(suffix)))
					{
						throw new Exception($"Error loading NPCEffect [{type.FullName}], it doesn't start with [{prefix}] and end with [{suffix}]");
					}
					else if (loadedTypeNamespaceToName.ContainsKey(type.Name))
					{
						throw new Exception($"Error loading NPCEffect [{type.FullName}], an effect with that same name already exists in [{loadedTypeNamespaceToName[type.Name]}]! Make sure to make effect names unique");
					}
					loadedTypeNamespaceToName.Add(type.Name, type.Namespace);

					NPCEffectTypes.Add(type);
				}
			}
		}

		public static void Unload()
		{
			NPCEffectTypes = null;
		}

		/// <summary>
		/// Type identified by sbyte (0 to 127)
		/// </summary>
		public static sbyte NPCEffectType(Type type)
		{
			return (sbyte)NPCEffectTypes.IndexOf(type);
		}

		public static Type GetNPCEffectType(int type)
		{
			return NPCEffectTypes[type];
		}

		/// <summary>
		/// Decrements each effects' time, and removes it if 0
		/// </summary>
		public static void UpdateStatus(NPC npc, RORGlobalNPC globalNPC)
		{
			for (int i = globalNPC.NPCEffects.Count - 1; i >= 0; i--)
			{
				NPCEffect effect = globalNPC.NPCEffects[i];
				if (effect.DecrementTime())
				{
					effect.OnRemove(npc);
					globalNPC.NPCEffects.Remove(effect);
				}
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
			return GetEffectIndexOfType(globalNPC, NPCEffectType(type));
		}

		public static int GetEffectIndexOfType(RORGlobalNPC globalNPC, int type)
		{
			return globalNPC.NPCEffects.FindIndex(e => e.Type == type);
		}

		/// <summary>
		/// Applies an effect of the given type to an npc. Only broadcasts if it's not on the npc before. If both booleans are true, it will do so anyway
		/// </summary>
		public static NPCEffect ApplyNPCEffect<T>(NPC npc, int duration, bool broadcast = false, bool forceBroadcast = false) where T : NPCEffect
		{
			return ApplyNPCEffect(typeof(T), npc, duration, broadcast, forceBroadcast);
		}

		/// <summary>
		/// Applies an effect of the given type to an npc. Only broadcasts if it's not on the npc before. If both booleans are true, it will do so anyway
		/// </summary>
		public static NPCEffect ApplyNPCEffect(int type, NPC npc, int duration, bool broadcast = false, bool forceBroadcast = false)
		{
			Type t = GetNPCEffectType(type);
			return ApplyNPCEffect(t, npc, duration, broadcast, forceBroadcast);
		}

		/// <summary>
		/// Applies an effect of the given type to an npc. Only broadcasts if it's not on the npc before. If both booleans are true, it will do so anyway
		/// </summary>
		public static NPCEffect ApplyNPCEffect(Type type, NPC npc, int duration, bool broadcast = false, bool forceBroadcast = false)
		{
			if (MiscManager.IsBuffImmune(npc)) return null;

			RORGlobalNPC globalNPC = npc.GetGlobalNPC<RORGlobalNPC>();
			int index = GetEffectIndexOfType(globalNPC, type);
			NPCEffect effect;

			if (index > -1)
			{
				//Effect exists
				effect = globalNPC.NPCEffects[index];
				effect.SetTime(duration);

				if (forceBroadcast && broadcast && Main.netMode != NetmodeID.SinglePlayer)
				{
					new NPCEffectPacket(npc, effect).Send();
				}
			}
			else
			{
				//Effect doesn't exist, add one
				effect = NPCEffect.CreateInstance(npc, type, duration);
				globalNPC.NPCEffects.Add(effect);

				if (broadcast && Main.netMode != NetmodeID.SinglePlayer)
				{
					new NPCEffectPacket(npc, effect).Send();
				}
			}
			return effect;
		}
	}
}
