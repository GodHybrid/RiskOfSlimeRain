using System.IO;
using WebmilioCommons.Networking;
using WebmilioCommons.Networking.Packets;

namespace RiskOfSlimeRain.Data.Warbanners
{
	public class WarbannerPacket : NetworkPacket
	{
		public override NetworkPacketBehavior Behavior => NetworkPacketBehavior.SendToServer;

		//Need to "wrap" variables for the packet to see them

		public int Radius
		{
			get => WarbannerManager.Radius;
			set => WarbannerManager.Radius = value;
		}

		public float X
		{
			get => WarbannerManager.X;
			set => WarbannerManager.X = value;
		}

		public float Y
		{
			get => WarbannerManager.Y;
			set => WarbannerManager.Y = value;
		}

		protected override bool PostReceive(BinaryReader reader, int fromWho)
		{
			//Do something with the received data, which is now in the variables we wrapped previously
			WarbannerManager.AddWarbanner();
			return base.PostReceive(reader, fromWho);
		}
	}
}
