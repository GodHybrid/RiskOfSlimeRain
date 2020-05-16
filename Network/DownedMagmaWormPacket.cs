using System.IO;
using WebmilioCommons.Networking;
using WebmilioCommons.Networking.Packets;

namespace RiskOfSlimeRain.Network
{
	/// <summary>
	/// Sent to all clients once Magma Worm is defeated
	/// </summary>
	public class DownedMagmaWormPacket : NetworkPacket
	{
		public override NetworkPacketBehavior Behavior => NetworkPacketBehavior.SendToAllClients;

		public DownedMagmaWormPacket() { }

		protected override bool PostReceive(BinaryReader reader, int fromWho)
		{
			RORWorld.downedMagmaWorm = true;
			return base.PostReceive(reader, fromWho);
		}
	}
}
