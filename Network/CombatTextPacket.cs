using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using WebmilioCommons.Networking;
using WebmilioCommons.Networking.Packets;

namespace RiskOfSlimeRain.Network
{
	public class CombatTextPacket : NetworkPacket
	{
		public override NetworkPacketBehavior Behavior => NetworkPacketBehavior.SendToAll;

		public uint PackedColor { get; set; }

		public string Text { get; set; }

		public float X { get; set; }

		public float Y { get; set; }

		public CombatTextPacket() { }

		private Color Color => new Color() { PackedValue = PackedColor };

		private Rectangle Position => new Rectangle((int)X, (int)Y, 1, 1);

		public CombatTextPacket(CombatText combatText) : this(combatText.position, combatText.color, combatText.text)
		{

		}

		public CombatTextPacket(Vector2 center, Color color, int number) : this(center, color, number.ToString())
		{

		}

		public CombatTextPacket(Vector2 center, Color color, string text)
		{
			X = center.X;
			Y = center.Y;
			PackedColor = color.PackedValue;
			Text = text;
		}

		protected override bool PostReceive(BinaryReader reader, int fromWho)
		{
			CombatText.NewText(Position, Color, Text, false, false);
			return base.PostReceive(reader, fromWho);
		}
	}
}
