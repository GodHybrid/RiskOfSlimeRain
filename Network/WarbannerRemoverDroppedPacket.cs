using RiskOfSlimeRain.Helpers;
using System.IO;
using Terraria;
using WebmilioCommons.Networking;
using WebmilioCommons.Networking.Packets;

namespace RiskOfSlimeRain.Network
{
	/// <summary>
	/// Sent to the client that got a Warbanner Remover dropped, simply sets the flag of that player to true
	/// </summary>
	public class WarbannerRemoverDroppedPacket : NetworkPacket
	{
		public override NetworkPacketBehavior Behavior => NetworkPacketBehavior.SendToClient;

		public int WhoAmI { get; set; }

		private Player Player => Main.player[WhoAmI];

		private RORPlayer ModPlayer => Player.GetRORPlayer();

		public WarbannerRemoverDroppedPacket() { }

		public WarbannerRemoverDroppedPacket(int whoAmI)
		{
			WhoAmI = whoAmI;
		}

		protected override bool PostReceive(BinaryReader reader, int fromWho)
		{
			ModPlayer.warbannerRemoverDropped = true;
			return base.PostReceive(reader, fromWho);
		}
	}
}
