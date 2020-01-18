using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace RiskOfSlimeRain.Data.NPCEffects
{
	/// <summary>
	/// Acts like a "buff", tied to RORGlobalNPC
	/// </summary>
	public class NPCEffect
	{
		public int Time { get; private set; }

		public string Name => GetType().Name;

		public bool RanOut { get; private set; }

		public NPCEffect()
		{

		}

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

		public void DecrementTime()
		{
			Time--;
			if (Time <= 0) RanOut = true;
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
		public static NPCEffect CreateInstance(Type type, int duration)
		{
			NPCEffect effect = (NPCEffect)Activator.CreateInstance(type);
			effect.SetTime(duration);
			return effect;
		}

		#region Hooks
		/// <summary>
		/// Ran in GlobalNPC.ResetEffects
		/// </summary>
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
