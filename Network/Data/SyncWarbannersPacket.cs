using RiskOfSlimeRain.Core.Warbanners;
using System.IO;

namespace RiskOfSlimeRain.Network.Data
{
	public class SyncWarbannersPacket : MPPacket
	{
		public SyncWarbannersPacket() { }

		public override void Send(BinaryWriter writer)
		{
			WarbannerManager.NetSend(writer);
		}

		public override void Receive(BinaryReader reader, int sender)
		{
			WarbannerManager.NetReceive(reader);
		}
	}
}
