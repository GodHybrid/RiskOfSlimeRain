using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ID;
using WebmilioCommons.Networking;
using WebmilioCommons.Networking.Packets;

namespace RiskOfSlimeRain.Network
{
	public class CombatTextPacket : NetworkPacket
	{
		public override NetworkPacketBehavior Behavior => NetworkPacketBehavior.SendToAllClients;

		public Vector2 Position { get; set; }

		public Color Color { get; set; }

		public string Text { get; set; }

		public byte Flags { get; set; }

		private BitsByte Bits => Flags;

		private Rectangle Location => new Rectangle((int)Position.X, (int)Position.Y, 0, 0);

		public CombatTextPacket() { }

		public CombatTextPacket(Rectangle location, Color color, string text, bool dramatic = false, bool dot = false)
		{
			Position = location.TopLeft();
			Color = color;
			Text = text;
			Flags = new BitsByte(b1: dramatic, b2: dot);
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

		protected override bool PostReceive(BinaryReader reader, int fromWho)
		{
			CombatText.NewText(Location, Color, Text, Bits[0], Bits[1]);
			return base.PostReceive(reader, fromWho);
		}
	}
}
