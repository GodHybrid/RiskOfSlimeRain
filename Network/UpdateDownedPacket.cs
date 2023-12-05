using RiskOfSlimeRain.Core.ItemSpawning.NPCSpawning;
using System.IO;
using Terraria.ModLoader;

namespace RiskOfSlimeRain.Network
{
	/// <summary>
	/// Sent to all clients once vanillaDowned or moddedDowned is changed
	/// </summary>
	public class UpdateDownedPacket/* : NetworkPacket*/
	{
		public void Send(int toWho = -1, int fromWho = -1) { }
		//public override NetworkPacketBehavior Behavior => NetworkPacketBehavior.SendToAllClients;

		public bool Vanilla { get; set; }

		public UpdateDownedPacket() { }

		/// <summary>
		/// Specify true to send the vanillaDowned, false for moddedDowned
		/// </summary>
		public UpdateDownedPacket(bool vanilla)
		{
			Vanilla = vanilla;
		}

		/*
		protected override bool PreSend(ModPacket modPacket, int? fromWho = null, int? toWho = null)
		{
			NPCLootManager.NetSend(modPacket, Vanilla);

			return base.PreSend(modPacket, fromWho, toWho);
		}

		protected override bool PostReceive(BinaryReader reader, int fromWho)
		{
			NPCLootManager.NetReceive(reader, Vanilla);

			return base.PostReceive(reader, fromWho);
		}*/
	}
}
