using Microsoft.Xna.Framework;
using RiskOfSlimeRain.Core.Warbanners;
using System.IO;
using Terraria;

namespace RiskOfSlimeRain.Network.Data
{
	public class WarbannerPacket : MPPacket
	{
		public readonly int radius;
		public readonly Vector2 position;

		public WarbannerPacket() { }

		public WarbannerPacket(int radius, Vector2 position)
		{
			this.radius = radius;
			this.position = position;
		}

		public override void Send(BinaryWriter writer)
		{
			writer.Write7BitEncodedInt(radius);
			writer.WriteVector2(position);
		}

		public override void Receive(BinaryReader reader, int sender)
		{
			int radius = reader.Read7BitEncodedInt();
			Vector2 position = reader.ReadVector2();
			WarbannerManager.AddWarbanner(radius, position.X, position.Y);
		}
	}
}
