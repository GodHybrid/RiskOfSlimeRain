using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ModLoader;
using WebmilioCommons.Networking.Packets;
using WebmilioCommons.Networking.Serializing;

namespace RiskOfSlimeRain.Network
{
	public static class RORNetworkTypeSerializers
	{
		public static void Load()
		{
			NetworkTypeSerializers.AddSerializer<CombatText>(new NetworkTypeSerializer(ReadCombatText, WriteCombatText));
		}

		//Rectangle location, Color color, string text, bool dramatic = false, bool dot = false

		public static object ReadCombatText(this NetworkPacket networkPacket, BinaryReader reader)
		{
			Vector2 position = reader.ReadPackedVector2();
			Color color = reader.ReadRGB();
			string text = reader.ReadString();
			BitsByte bits = reader.ReadByte();
			return CombatText.NewText(new Rectangle((int)position.X, (int)position.Y, 0, 0), color, text, bits[0], bits[1]);
		}

		public static void WriteCombatText(this NetworkPacket networkPacket, ModPacket modPacket, object value)
		{
			CombatText combatText = (CombatText)value;
			modPacket.WritePackedVector2(combatText.position);
			modPacket.WriteRGB(combatText.color);
			modPacket.Write(combatText.text);
			BitsByte bits = new BitsByte(b1: combatText.crit, b2: combatText.dot);
			modPacket.Write(bits);
		}
	}
}
