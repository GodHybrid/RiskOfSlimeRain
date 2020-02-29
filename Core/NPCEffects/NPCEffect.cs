using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;

namespace RiskOfSlimeRain.Core.NPCEffects
{
	/// <summary>
	/// Acts like a "buff", tied to RORGlobalNPC
	/// </summary>
	public class NPCEffect
	{
		public int Time { get; private set; }

		public string Name => GetType().Name;

		public sbyte Type => NPCEffectManager.NPCEffectType(GetType());

		//Because we are using NPCEffect in a list, we need those two
		public override bool Equals(object obj)
		{
			if (obj is NPCEffect other)
				return Time == other.Time && Name == other.Name;
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return new { Time, Name }.GetHashCode();
		}

		public override string ToString() => $"{Name}; {Time}";

		/// <summary>
		/// Returns true if the timer reached 0
		/// </summary>
		public bool DecrementTime()
		{
			Time--;
			if (Time <= 0) return true;
			return false;
		}

		//TODO add docs

		public virtual void OnRemove(NPC npc)
		{

		}

		public virtual void Init(NPC npc)
		{

		}

		public virtual void NetSend(BinaryWriter writer)
		{

		}

		public virtual void NetReceive(BinaryReader reader)
		{

		}

		/// <summary>
		/// Sets the duration of the buff to this amount if it's below
		/// </summary>
		public void SetTime(int duration)
		{
			if (Time < duration) Time = duration;
		}

		/// <summary>
		/// Creates an NPCEffect with set duration
		/// </summary>
		public static NPCEffect CreateInstance(NPC npc, Type type, int duration)
		{
			NPCEffect effect = (NPCEffect)Activator.CreateInstance(type);
			effect.SetTime(duration);
			effect.Init(npc);
			return effect;
		}

		#region Hooks
		public virtual void AI(NPC npc)
		{

		}

		public virtual void PostDraw(NPC npc, SpriteBatch spriteBatch, Color drawColor)
		{

		}

		public virtual void DrawEffects(NPC npc, ref Color drawColor)
		{

		}
		#endregion
	}
}
