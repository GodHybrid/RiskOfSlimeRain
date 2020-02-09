using Terraria;
using WebmilioCommons.Networking;
using WebmilioCommons.Networking.Packets;

namespace RiskOfSlimeRain.Network
{
	public class CombatTextPacket : NetworkPacket
	{
		public override NetworkPacketBehavior Behavior => NetworkPacketBehavior.SendToAllClients;

		public CombatText CombatText { get; set; }

		public CombatTextPacket() { }

		public CombatTextPacket(int index) : this(Main.combatText[index]) { }

		public CombatTextPacket(CombatText combatText)
		{
			CombatText = combatText;
		}

		//public Color Color { get; set; }

		//public string Text { get; set; }

		//public Vector2 Position { get; set; }

		//public CombatTextPacket() { }

		//private Rectangle Location => new Rectangle((int)Position.X, (int)Position.Y, 1, 1);

		//public CombatTextPacket(CombatText combatText) : this(combatText.position, combatText.color, combatText.text)
		//{

		//}

		//public CombatTextPacket(Vector2 center, Color color, int number) : this(center, color, number.ToString())
		//{

		//}

		//public CombatTextPacket(Vector2 center, Color color, string text)
		//{
		//	Position = center;
		//	Color = color;
		//	Text = text;
		//}

		//protected override bool PostReceive(BinaryReader reader, int fromWho)
		//{
		//	CombatText.NewText(Location, Color, Text, false, false);
		//	return base.PostReceive(reader, fromWho);
		//}
	}
}
