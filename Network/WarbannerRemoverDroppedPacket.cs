using WebmilioCommons.Networking;
using WebmilioCommons.Networking.Packets;

namespace RiskOfSlimeRain.Network
{
	public class WarbannerRemoverDroppedPacket : ModPlayerNetworkPacket<RORPlayer>
	{
		public override NetworkPacketBehavior Behavior => NetworkPacketBehavior.SendToClient;

		public bool WarbannerRemoverDropped
		{
			get => base.ModPlayer.warbannerRemoverDropped;
			set => base.ModPlayer.warbannerRemoverDropped = value;
		}

		public WarbannerRemoverDroppedPacket() { }
	}
}
