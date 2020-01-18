using Microsoft.Xna.Framework;
using System;
using System.IO;
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

		public Warbanner(int radius, Vector2 position)
		{
			this.radius = radius;
			this.position = position;
		}

		public override string ToString() => "Radius (tiles): " + (radius >> 4) + "; Position: " + position;

		//From TagSerializable, required explicitely
		public static Warbanner Load(TagCompound tag)
		{
			return new Warbanner(tag.GetInt("radius"), tag.Get<Vector2>("position"));
		}

		//From TagSerializable, required implicitely
		public TagCompound SerializeData()
		{
			return new TagCompound {
				{"radius", radius },
				{"position", position },
			};
		}

		public void NetRecieve(BinaryReader reader)
		{
			radius = reader.ReadInt32();
			position.X = reader.ReadSingle();
			position.Y = reader.ReadSingle();
		}

		public void NetSend(BinaryWriter writer)
		{
			writer.Write((int)radius);
			writer.Write((float)position.X);
			writer.Write((float)position.Y);
		}
	}
}
