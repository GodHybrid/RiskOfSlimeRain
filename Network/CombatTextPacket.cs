using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;

namespace RiskOfSlimeRain.Network
{
	public class CombatTextPacket : MPPacket
	{
		public readonly Vector2 position;
		public readonly Color color;
		public readonly string text;
		public readonly byte flags;

		public CombatTextPacket() { }

		public CombatTextPacket(Vector2 position, Color color, string text, byte flags)
		{
			this.position = position;
			this.color = color;
			this.text = text;
			this.flags = flags;
		}

		public CombatTextPacket(Rectangle location, Color color, string text, bool dramatic = false, bool dot = false) : this(location.TopLeft(), color, text, new BitsByte(b1: dramatic, b2: dot))
		{

		}

		/// <summary>
		/// <seealso cref="CombatText.NewText(Rectangle, Color, int, bool, bool)"/> but with syncing
		/// </summary>
		public static int NewText(Rectangle location, Color color, int number, bool dramatic = false, bool dot = false)
		{
			return NewText(location, color, number.ToString(), dramatic, dot);
		}

		/// <summary>
		/// <seealso cref="CombatText.NewText(Rectangle, Color, string, bool, bool)"/> but with syncing
		/// </summary>
		public static int NewText(Rectangle location, Color color, string text, bool dramatic = false, bool dot = false)
		{
			int index = CombatText.NewText(location, color, text, dramatic, dot);
			if (Main.netMode == NetmodeID.MultiplayerClient && index != Main.maxCombatText)
			{
				new CombatTextPacket(location, color, text, dramatic, dot).Send();
			}
			return index;
		}

		public override void Send(BinaryWriter writer)
		{
			writer.WriteVector2(position);
			writer.WriteRGB(color);
			writer.Write((string)text);
			writer.Write((byte)flags);
		}

		public override void Receive(BinaryReader reader, int sender)
		{
			var position = reader.ReadVector2();
			var color = reader.ReadRGB();
			var text = reader.ReadString();
			var flags = (BitsByte)reader.ReadByte();

			if (Main.netMode == NetmodeID.MultiplayerClient)
			{
				CombatText.NewText(new Rectangle((int)position.X, (int)position.Y, 0, 0), color, text, flags[0], flags[1]);
			}
			else
			{
				new CombatTextPacket(position, color, text, flags).Send(from: sender);
			}
		}
	}
}
