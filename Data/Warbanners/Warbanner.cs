using Microsoft.Xna.Framework;
using System;
using Terraria.ModLoader.IO;

namespace RiskOfSlimeRain.Data.Warbanners
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
				return radius == other.radius && position == other.position;
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return new { radius, position }.GetHashCode();
		}

		//From TagSerializable, required explicitely
		public static readonly Func<TagCompound, Warbanner> DESERIALIZER = Load;

		public int radius;

		public Vector2 position;

		/// <summary>
		/// Used to play a sound if the warbanner does not originate from the previous session
		/// </summary>
		public bool fresh;

		public Warbanner() { }

		public Warbanner(int radius, Vector2 position, bool fresh = true)
		{
			this.radius = radius;
			this.position = position;
			this.fresh = fresh;
		}

		public Warbanner(int radius, float x, float y, bool fresh = true) : this(radius, new Vector2(x, y), fresh) { }

		public override string ToString() => "Radius (tiles): " + (radius >> 4) + "; Position: " + position;

		//From TagSerializable, required explicitely
		public static Warbanner Load(TagCompound tag)
		{
			return new Warbanner(tag.GetInt("radius"), tag.Get<Vector2>("position"), fresh: false);
		}

		//From TagSerializable, required implicitely
		public TagCompound SerializeData()
		{
			return new TagCompound {
				{"radius", radius },
				{"position", position },
			};
		}
	}
}
