using RiskOfSlimeRain.Core.ItemSpawning.NPCSpawning;
using System.IO;

namespace RiskOfSlimeRain.Network
{
	/// <summary>
	/// Sent to all clients once vanillaDowned or moddedDowned is changed
	/// </summary>
	public class UpdateDownedPacket : MPPacket
	{
		public readonly bool vanilla;

		public UpdateDownedPacket() { }

		/// <summary>
		/// Specify true to send the vanillaDowned, false for moddedDowned
		/// </summary>
		public UpdateDownedPacket(bool vanilla)
		{
			this.vanilla = vanilla;
		}

		public override void Send(BinaryWriter writer)
		{
			writer.Write((bool)vanilla);
			NPCLootManager.NetSend(writer, vanilla);
		}

		public override void Receive(BinaryReader reader, int sender)
		{
			bool vanilla = reader.ReadBoolean();
			NPCLootManager.NetReceive(reader, vanilla);
		}
	}
}
