using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ModLoader.IO;

namespace RiskOfSlimeRain.Core.Warbanners
{
	/// <summary>
	/// Convenience class to save data about a warbanner
	/// </summary>
	public sealed class Warbanner : TagSerializable
	{
		//because we are using Warbanner in a list, we need those two
		public override bool Equals(object obj)
		{
			if (obj is Warbanner other)
				return radius == other.radius && position == other.position && timeLeft == other.timeLeft;
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return new { radius, position, timeLeft }.GetHashCode();
		}

		//From TagSerializable, required explicitely
		public static readonly Func<TagCompound, Warbanner> DESERIALIZER = Load;

		public int radius;

		public Vector2 position;

		/// <summary>
		/// -1 if infinite. Decrements every tick and disappears once reaching 0
		/// </summary>
		public int timeLeft;

		/// <summary>
		/// Used to play a sound if the warbanner does not originate from the previous session
		/// </summary>
		public bool fresh;

		public int associatedProjIdentity = -1;

		public Warbanner() { }

		public Warbanner(int radius, Vector2 position, int timeLeft, bool fresh = true)
		{
			this.radius = radius;
			this.position = position;
			this.timeLeft = timeLeft;
			this.fresh = fresh;
		}

		public Warbanner(int radius, float x, float y, int timeLeft, bool fresh = true) : this(radius, new Vector2(x, y), timeLeft, fresh) { }

		public override string ToString() => $"Radius (tiles): {radius * 16}; Position: {position}; TimeLeft: {timeLeft}; Fresh: {fresh}";

		//From TagSerializable, required explicitely
		public static Warbanner Load(TagCompound tag)
		{
			if (tag.ContainsKey("timeLeft"))
			{
				return new Warbanner(tag.GetInt("radius"), tag.Get<Vector2>("position"), tag.GetInt("timeLeft"), fresh: false);
			}

			return new Warbanner(tag.GetInt("radius"), tag.Get<Vector2>("position"), -1, fresh: false);
		}

		//From TagSerializable, required implicitely
		public TagCompound SerializeData()
		{
			return new TagCompound {
				{"radius", radius },
				{"position", position },
				{"timeLeft", timeLeft },
			};
		}

		public void NetSend(BinaryWriter writer)
		{
			writer.Write7BitEncodedInt(radius);
			writer.WriteVector2(position);
			writer.Write7BitEncodedInt(timeLeft);
		}

		public static Warbanner FromReader(BinaryReader reader)
		{
			int radius = reader.Read7BitEncodedInt();
			Vector2 position = reader.ReadVector2();
			int timeLeft = reader.Read7BitEncodedInt();

			return new Warbanner(radius, position, timeLeft, fresh: false);
		}
	}
}
